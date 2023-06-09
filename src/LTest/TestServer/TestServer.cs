using LTest.Exceptions;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using LTest.LogSniffer;
using LTest.Services;
using LTest.TestServer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// WebApplication factory base class for integration tests.
    /// </summary>
    /// <typeparam name="TStartup">Startup class.</typeparam>
    public class TestServer<TStartup> : WebApplicationFactory<TStartup>, ITestServer
        where TStartup : class
    {
        private readonly TestLogger _logger = new();
        private readonly Action<ServerConfigurator>? _configAction;

        private string? _serverName;
        private ServerConfigurator _configurator;
        private HttpClient _httpClient;
        private LTestFacade _facade;
        private HookRunner _hookRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServer{TStartup}"/> class.
        /// </summary>
        public TestServer(Action<ServerConfigurator>? configAction = null)
        {
            _configAction = configAction;
        }

        private bool StartServer(string serverName)
        {
            if (_serverName != null)
                return false;

            _serverName = serverName;
            _configurator = InitConfigurator();
            _configAction?.Invoke(_configurator);

            using var scope = _logger.Scope(serverName);
            _logger.LogInformation($"Starting server '{GetType().Name}'");
            var result = StopwatchHelper.Measure(() => CreateClient(_configurator.HttpClientOptions));
            _logger.LogInformation($"Server started ({result.ElapsedMilliseconds} ms)");

            _httpClient = result.ResultObject;

            return true;
        }

        public async Task<LTestFacade> InitScopeAsync(string serverName)
        {
            var started = StartServer(serverName);

            _facade = new LTestFacade(Services, _httpClient);
            _hookRunner = _facade.GetRequiredService<HookRunner>();

            if (started)
            {
                await _hookRunner.RunHooksAsync<IAfterServerStartedHook>(x => x.AfterServerStartedAsync());
            }
            else
            {
                //await _hookHelper.RunHooksAsync<IResetSingletonHook>(x => x.ResetAsync());
                using var scope = _logger.Scope(serverName);
                _logger.LogInformation("Server is already running");
            }

            await _hookRunner.RunHooksAsync<IBeforeTestHook>(x => x.BeforeTestAsync());

            return _facade;
        }

        public async Task CleanUpAsync(ITestOutputHelper output)
        {
            try
            {
                await _hookRunner.RunHooksAsync<IAfterTestHook>(x => x.AfterTestAsync());

                FlushLogger(output);

                var openScopes = _logger.Scopes.Where(x => !x.IsDisposed).ToList();
                if (openScopes.Count != 0)
                {
                    throw new InvalidOperationException($"These logger scopes were not disposed: {string.Join(", ", openScopes.Select(x => JsonSerializer.Serialize(x.State)))}");
                }

                _logger.Clear();

                var serverLogs = _facade.LogSniffer.GetServerLogs();
                if (serverLogs.Any(x => x.IsUnexpected))
                {
                    throw new BulletProveException("Unexpected log occured on server side. Check the logs!");
                }
            }
            finally
            {
                await _facade.DisposeAsync();
            }
        }

        private void FlushLogger(ITestOutputHelper output)
        {
            var defaultGroupId = Guid.NewGuid().ToString();

            var groups = _logger.GetSnapshot()
                .GroupBy(x => x.Scope == null ? defaultGroupId : x.Scope.GroupId)
                .OrderBy(group => group.Min(log => log.CreatedAt));

            foreach (var group in groups)
            {
                output.WriteLine($"({group.Key})");

                foreach (var logEvent in group)
                {
                    if (logEvent.Level == LogLevel.None)
                    {
                        output.WriteLine(string.Empty);
                    }
                    else
                    {
                        var indent = logEvent.Scope == null ? 0 : logEvent.Scope.Level * 2;
                        output.WriteLine($"{logEvent.Level.ToString()[0]}: {new string(' ', indent)}{logEvent.Message}");
                    }
                }

                output.WriteLine(string.Empty);
            }
        }

        /// <summary>
        /// ConfigureWebHost.
        /// </summary>
        /// <param name="builder">IWebHostBuilder.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var (key, value) in _configurator.Settings)
            {
                builder.UseSetting(key, value);
            }

            if (_configurator.JsonConfigurationFiles.Count > 0)
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    var root = Directory.GetCurrentDirectory();
                    var fileProvider = new PhysicalFileProvider(root);

                    foreach (var file in _configurator.JsonConfigurationFiles)
                    {
                        config.AddJsonFile(fileProvider, file, false, false);
                    }
                });
            }

            builder.ConfigureLogging((_, logging) =>
            {
                logging.Services.RemoveAll<ILoggerFactory>();
                logging.Services.RemoveAll<ILoggerProvider>();
                logging.Services.AddSingleton<ILoggerFactory, LTestLoggerFactory>();
            });

            builder.ConfigureTestServices(services =>
            {
                foreach (var serviceConfigurator in _configurator.ServiceConfigurators)
                {
                    serviceConfigurator(services);
                }
            });
        }

        /// <summary>
        /// Registers the LTest services.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterLTestServices(IServiceCollection services)
        {
            // Configuration
            services.AddSingleton(_configurator);

            // Logger
            services.AddSingleton<ITestLogger>(_logger);

            // LogSniffer
            services.AddSingleton<ILogSnifferService, DefaultServerLogInspector>();
            services.AddSingleton<IServerLogInspector>(sp => (DefaultServerLogInspector)sp.GetRequiredService<ILogSnifferService>());

            // DisposableCollector
            services.AddScoped<DisposableCollertor>();

            // Hooks
            services.AddTransient<HookRunner>();
        }

        /// <summary>
        /// Registers the reset singleton hooks.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterResetSingletonHooks(IServiceCollection services)
        {
            var type = typeof(IResetSingletonHook);
            var resetHookServices = services
                .Where(x => (x.ImplementationType?.IsAssignableTo(type) ?? false) || (x.ImplementationInstance?.GetType().IsAssignableTo(type) ?? false))
                .ToList();

            foreach (var service in resetHookServices)
            {
                if (service.Lifetime != ServiceLifetime.Singleton)
                {
                    throw new InvalidOperationException($"{nameof(IResetSingletonHook)} can be used only with singletons, but {service.ImplementationType!.Name} was registered as {service.Lifetime}");
                }

                if (service.ServiceType != null)
                {
                    services.AddSingleton(sp => (IResetSingletonHook)sp.GetRequiredService(service.ServiceType));
                }
                else if (service.ImplementationType != null)
                {
                    services.AddSingleton(sp => (IResetSingletonHook)sp.GetRequiredService(service.ImplementationType!));
                }
            }
        }

        private static string? GetAppName()
        {
            var startupType = typeof(TStartup);

            var appName = startupType.Namespace?.Split('.')[0]
                ?? startupType.Assembly.FullName?.Split(',')[0].Split('.')[0];

            return appName;
        }

        /// <summary>
        /// Inits the configuration.
        /// </summary>
        /// <returns>An LTestConfiguration.</returns>
        private ServerConfigurator InitConfigurator()
        {
            var configurator = new ServerConfigurator()
                .AddSetting("https_port", "443")
                .ConfigureTestServices(services =>
                {
                    RegisterLTestServices(services);
                    RegisterResetSingletonHooks(services);
                });

            var appName = GetAppName();
            if (!string.IsNullOrWhiteSpace(appName))
            {
                configurator.ConfigureLoggerCategoryNameInspector(inspector =>
                {
                    inspector.AddDefaultAllowedAction(x => x.StartsWith(appName, StringComparison.OrdinalIgnoreCase), "AppName");
                });
            }

            configurator.ConfigureServerLogInspector(inspector =>
            {
                inspector.AddDefaultAllowedAction(logEvent => logEvent.Level < LogLevel.Warning, "LogLevelBelowWarning");
            });

            return configurator;
        }
    }
}