using BulletProve.Base.Configuration;
using BulletProve.Base.Hooks;
using BulletProve.Exceptions;
using BulletProve.Helpers;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BulletProve
{
    /// <summary>
    /// WebApplication factory base class for integration tests.
    /// </summary>
    /// <typeparam name="TStartup">Startup class.</typeparam>
    public class TestServer<TStartup> : WebApplicationFactory<TStartup>, ITestServer
        where TStartup : class
    {
        private readonly Action<ServerConfigurator>? _configAction;
        private readonly ScopeProvider _scopeProvider;
        private readonly TestLogger _logger;

        private string? _serverName;
        private LoggerConfigurator _loggerConfigurator = null!;
        private ServerConfigurator _serverConfigurator = null!;
        private HttpClient _httpClient = null!;
        private ServerScope _scope = null!;
        private IHookRunner _hookRunner = null!;
        private bool _isInitialized = false;

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServer"/> class.
        /// </summary>
        /// <param name="configAction">The config action.</param>
        public TestServer(Action<ServerConfigurator>? configAction)
        {
            _configAction = configAction;
            _scopeProvider = new ScopeProvider();
            _logger = new TestLogger(_scopeProvider);
        }

        /// <inheritdoc />
        public async Task<IServerScope> StartSessionAsync(string serverName)
        {
            var started = StartServer(serverName);

            _scope = new ServerScope(Services, _httpClient);
            _hookRunner = _scope.GetRequiredService<IHookRunner>();

            if (started)
            {
                await _hookRunner.RunHooksAsync<IAfterServerStartedHook>(async x => await x.AfterServerStartedAsync());
            }
            else
            {
                using var scope = _logger.Scope(serverName);
                _logger.LogInformation("Server is already running");
            }

            await _hookRunner.RunHooksAsync<IBeforeTestHook>(async x => await x.BeforeTestAsync());

            _isInitialized = true;
            return _scope;
        }

        /// <inheritdoc />
        public async Task EndSessionAsync(IOutput output)
        {
            if (!_isInitialized)
                return;

            try
            {
                await _hookRunner.RunHooksAsync<IAfterTestHook>(async x => await x.AfterTestAsync());

                FlushLogger(output);

                var openScopes = _scopeProvider.GetOpenScopes();
                var serverLogs = _scope.ServerLogCollector.GetServerLogs();

                await _hookRunner.RunHooksAsync<ICleanUpHook>(async x => await x.CleanUpAsync());
                
                if (openScopes.Count != 0)
                {
                    throw new BulletProveException($"These logger scopes were not disposed: {string.Join(", ", openScopes.Select(x => JsonSerializer.Serialize(x.State)))}");
                }

                if (serverLogs.Any(x => x.IsUnexpected))
                {
                    throw new BulletProveException("Unexpected log occured on server side. Check the logs!");
                }
            }
            finally
            {
                await _scope.DisposeAsync();
            }
        }

        /// <summary>
        /// Starts the server if not running already.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        private bool StartServer(string serverName)
        {
            if (_serverName != null)
                return false;

            _serverName = serverName;
            _serverConfigurator = InitConfigurator();
            _loggerConfigurator = _serverConfigurator.LoggerConfigurator;
            _configAction?.Invoke(_serverConfigurator);

            using var scope = _logger.Scope(serverName);
            _logger.LogInformation("Starting server");
            var result = StopwatchHelper.Measure(() => CreateClient(_serverConfigurator.HttpClientOptions));
            _logger.LogInformation($"Server started ({result.ElapsedMilliseconds} ms)");

            _httpClient = result.ResultObject;

            return true;
        }


        /// <summary>
        /// Flushes the logger.
        /// </summary>
        /// <param name="output">The output.</param>
        private void FlushLogger(IOutput output)
        {
            var defaultGroupId = Guid.NewGuid().ToString();

            var groups = _logger.GetSnapshot()
                .GroupBy(x => x.Scope == null ? defaultGroupId : x.Scope.GroupId)
                .OrderBy(group => group.Min(log => log.CreatedAt));

            foreach (var group in groups)
            {
                Scope? scope = null;
                var checkedScopes = new HashSet<Scope>();

                foreach (var logEvent in group)
                {
                    if (logEvent.Level == LogLevel.None)
                    {
                        output.WriteLine(string.Empty);
                        continue;
                    }

                    if (scope != logEvent.Scope)
                    {
                        LogScope(logEvent.Scope, checkedScopes, output);
                        scope = logEvent.Scope;
                    }

                    WriteLog(output, logEvent);
                }

                output.WriteLine(string.Empty);
            }
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="logEvent">The log event.</param>
        private void WriteLog(IOutput output, TestLogEvent logEvent)
        {
            var prefix = logEvent.Category == TestLogger.Category ? 'T' : 'S';
            var unexpected = logEvent.IsExpected ? ' ' : 'U';
            var level = logEvent.Level.ToString()[0];
            var indent = new string(' ', logEvent.Scope == null ? 0 : logEvent.Scope.Level * 2);
            var message = logEvent.Message.Replace(Environment.NewLine, $"{Environment.NewLine}     {indent}");

            if (_loggerConfigurator.LogCategoryNames)
                output.WriteLine($"{prefix}{unexpected}{level}: {indent}{message} - {logEvent.Category}");
            else
                output.WriteLine($"{prefix}{unexpected}{level}: {indent}{message}");
        }

        /// <summary>
        /// Logs the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="checkedScopes">The checked scopes.</param>
        /// <param name="output">The output.</param>
        private void LogScope(Scope? scope, HashSet<Scope> checkedScopes, IOutput output)
        {
            if (scope == null || checkedScopes.Contains(scope))
                return;

            if (scope.Parent != null)
                LogScope(scope.Parent, checkedScopes, output);

            if (scope.IsHelperScope)
                return;

            if (_loggerConfigurator.LoggerCategoryNameInspector.IsAllowed(scope.Category))
            {
                var indent = new string(' ', scope.Level * 2);
                if (_loggerConfigurator.LogCategoryNames)
                    output.WriteLine($"SCO: {indent}{scope} - {scope.Category}");
                else
                    output.WriteLine($"SCO: {indent}{scope}");
            }

            checkedScopes.Add(scope);
        }

        /// <inheritdoc />
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var (key, value) in _serverConfigurator.AppSettings)
            {
                builder.UseSetting(key, value);
            }

            if (_serverConfigurator.JsonConfigurationFiles.Count > 0)
            {
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    var root = Directory.GetCurrentDirectory();
                    var fileProvider = new PhysicalFileProvider(root);

                    foreach (var file in _serverConfigurator.JsonConfigurationFiles)
                    {
                        config.AddJsonFile(fileProvider, file, false, false);
                    }
                });
            }

            builder.ConfigureLogging((_, logging) =>
            {
                logging.Services.RemoveAll<ILoggerFactory>();
                logging.Services.RemoveAll<ILoggerProvider>();
                logging.Services.AddSingleton<ILoggerFactory, TestLoggerFactory>();
            });

            builder.ConfigureTestServices(services =>
            {
                foreach (var serviceConfigurator in _serverConfigurator.ServiceConfigurators)
                {
                    serviceConfigurator(services);
                }

                RegisterResetSingletonHooks(services);
            });
        }

        /// <summary>
        /// Registers the test services.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterTestServices(IServiceCollection services)
        {
            var loggerConfigurator = _serverConfigurator.LoggerConfigurator;

            // Configuration
            services.AddSingleton(_serverConfigurator);
            services.AddSingleton(loggerConfigurator);

            // Logger
            services.AddSingleton(_scopeProvider);
            services.AddSingleton<ITestLogger>(_logger);
            services.AddSingleton<IServerLogCollector, ServerLogCollector>();
            services.AddSingleton<IServerLogHandler, DefaultServerLogHandler>();

            // DisposableCollector
            services.AddScoped<DisposableCollector>();

            // Hooks
            services.AddTransient<IHookRunner, HookRunner>();
        }

        /// <summary>
        /// Registers the reset singleton hooks.
        /// </summary>
        /// <param name="services">The services.</param>
        private void RegisterResetSingletonHooks(IServiceCollection services)
        {
            var type = typeof(ICleanUpHook);
            var resetHookServices = services
                .Where(x => (x.ImplementationType?.IsAssignableTo(type) ?? false) || (x.ImplementationInstance?.GetType().IsAssignableTo(type) ?? false))
                .ToList();

            foreach (var service in resetHookServices)
            {
                if (service.Lifetime != ServiceLifetime.Singleton)
                    throw new BulletProveException($"{nameof(ICleanUpHook)} can be used only with singletons, but {service.ImplementationType!.Name} was registered as {service.Lifetime}");

                if (service.ServiceType == type)
                    continue;

                if (service.ImplementationInstance != null)
                    services.AddSingleton((ICleanUpHook)service.ImplementationInstance);
                else
                    services.AddSingleton(sp => (ICleanUpHook)sp.GetRequiredService(service.ServiceType));
            }
        }

        /// <summary>
        /// Inits the configuration.
        /// </summary>
        private ServerConfigurator InitConfigurator()
        {
            var configurator = new ServerConfigurator();

            configurator
                .UseAppSetting("https_port", "443")
                .ConfigureTestServices(services =>
                {
                    RegisterTestServices(services);
                })
                .ConfigureLogger(logger =>
                {
                    var appName = GetAppName();
                    if (!string.IsNullOrWhiteSpace(appName))
                    {
                        logger.ConfigureAllowedLoggerCategoryNames(inspector =>
                        {
                            inspector.AddDefaultAllowedAction(x => x.StartsWith(appName, StringComparison.OrdinalIgnoreCase), "AppName");
                            inspector.AddDefaultAllowedAction(x => x == TestLogger.Category, "TestLogger");
                        });
                    }

                    logger.ConfigureServerLogInspector(inspector =>
                    {
                        inspector.AddDefaultAllowedAction(logEvent => logEvent.Level < LogLevel.Warning, "LogLevelBelowWarning");
                    });
                });

            return configurator;
        }

        /// <summary>
        /// Gets the app name.
        /// </summary>
        private static string? GetAppName()
        {
            var startupType = typeof(TStartup);

            var appName = startupType.Namespace?.Split('.')[0]
                ?? startupType.Assembly.FullName?.Split(',')[0].Split('.')[0];

            return appName;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            IsDisposed = true;
        }
    }
}