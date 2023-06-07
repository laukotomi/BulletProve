using LTest.Configuration;
using LTest.Exceptions;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using LTest.LogSniffer;
using LTest.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// WebApplication factory base class for integration tests.
    /// </summary>
    /// <typeparam name="TStartup">Startup class.</typeparam>
    public abstract class TestServerBase<TStartup> : WebApplicationFactory<TStartup>, ITestServer
        where TStartup : class
    {
        private readonly LTestConfiguration _configuration;
        private readonly TestLogger _logger;

        private HttpClient _httpClient;
        private LTestFacade _facade;
        private string? _serverName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServerBase{TStartup}"/> class.
        /// </summary>
        protected TestServerBase()
        {
            _configuration = InitConfiguration();
            Configure(_configuration);
            _logger = new TestLogger();
        }

        private bool StartServer(string serverName)
        {
            if (_serverName != null)
                return false;

            _serverName = serverName;

            using var scope = _logger.Scope(serverName);
            _logger.LogInformation($"Starting server '{GetType().Name}'");
            var result = StopwatchHelper.Measure(() => CreateClient(_configuration.WebApplicationFactoryClientOptions));
            _logger.LogInformation($"Server started ({result.ElapsedMilliseconds} ms)");

            _httpClient = result.ResultObject;

            return true;
        }

        public async Task<LTestFacade> InitScopeAsync(string serverName)
        {
            var started = StartServer(serverName);

            _facade = new LTestFacade(Services, _httpClient);

            if (started)
            {
                await HookHelper.RunHooksAsync<IAfterServerStartedHook>(_facade, x => x.AfterServerStartedAsync());
            }
            else
            {
                //await HookHelper.RunHooksAsync<IResetSingletonHook>(_facade, x => x.ResetAsync());
                using var scope = _logger.Scope(serverName);
                _logger.LogInformation("Server is already running");
            }

            await HookHelper.RunHooksAsync<IBeforeTestHook>(_facade, x => x.BeforeTestAsync());

            return _facade;
        }

        public async Task CleanUpAsync(ITestOutputHelper output)
        {
            try
            {
                await HookHelper.RunHooksAsync<IAfterTestHook>(_facade, x => x.AfterTestAsync());

                FlushLogger(output);

                var openScopes = _logger.Scopes.Where(x => !x.IsDisposed).ToList();
                if (openScopes.Count != 0)
                {
                    throw new InvalidOperationException($"These logger scopes were not disposed: {string.Join(", ", openScopes.Select(x => JsonSerializer.Serialize(x.State)))}");
                }

                _logger.Clear();

                if (_facade.LogSniffer.UnexpectedLogOccured)
                {
                    throw new LogSnifferException("Unexpected log occured on server side. Check the logs!");
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
        /// Configure test services in this method.
        /// </summary>
        /// <param name="services">Service collection.</param>
        protected abstract void ConfigureTestServices(IServiceCollection services);

        /// <summary>
        /// Configure parameters here.
        /// </summary>
        /// <param name="config">Configuration.</param>
        protected abstract void Configure(LTestConfiguration config);

        /// <summary>
        /// ConfigureWebHost.
        /// </summary>
        /// <param name="builder">IWebHostBuilder.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("https_port", "443");

            builder
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var root = Directory.GetCurrentDirectory();
                    var fileProvider = new PhysicalFileProvider(root);
                    builder.AddJsonFile(fileProvider, "integrationtestsettings.json", true, false);
                    builder.AddJsonFile(fileProvider, "integrationtestsettings.Development.json", true, false);

                    foreach (var file in _configuration.ConfigurationFiles)
                    {
                        builder.AddJsonFile(fileProvider, file, false, false);
                    }
                })
                .ConfigureLogging((context, builder) =>
                {
                    if (!_configuration.PreserveLoggerProviders)
                    {
                        builder.Services.RemoveAll<ILoggerFactory>();
                        builder.Services.RemoveAll<ILoggerProvider>();
                        builder.Services.AddSingleton<ILoggerFactory, LTestLoggerFactory>();
                    }
                    else
                    {
                        _logger.LogWarning($"{nameof(LTestLogger)} registration was skipped.");
                    }
                })
                .ConfigureTestServices(services =>
                {
                    RegisterLTestServices(services);
                    ConfigureTestServices(services);
                    RegisterResetSingletonHooks(services);
                });
        }

        /// <summary>
        /// Inits the configuration.
        /// </summary>
        /// <returns>An LTestConfiguration.</returns>
        private static LTestConfiguration InitConfiguration()
        {
            var startupType = typeof(TStartup);
            LogFilter<string> logFilter;

            var appName = startupType.Namespace?.Split('.')[0]
                ?? startupType.Assembly.FullName?.Split(',')[0].Split('.')[0];

            if (!string.IsNullOrEmpty(appName))
            {
                var filter = new LogEventFilter<string>("AppName", x => x.StartsWith(appName, StringComparison.OrdinalIgnoreCase));
                logFilter = new LogFilter<string>(new[] { filter });
            }
            else
            {
                logFilter = new LogFilter<string>();
            }

            return new LTestConfiguration(logFilter);
        }

        /// <summary>
        /// Registers the LTest services.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterLTestServices(IServiceCollection services)
        {
            // Configuration
            services.AddSingleton(_configuration);

            // Logger
            services.AddSingleton<ITestLogger>(_logger);

            // LogSniffer
            services.AddSingleton<ILogSnifferService, LogSnifferService>();

            // DisposableCollector
            services.AddScoped<DisposableCollertor>();
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
    }
}