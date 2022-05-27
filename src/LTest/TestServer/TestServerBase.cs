using LTest.Configuration;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Http;
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
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServerBase{TStartup}"/> class.
        /// </summary>
        protected TestServerBase()
        {
            _configuration = InitConfiguration();
            Configure(_configuration);
            _logger = new TestLogger(_configuration);

            using var loggerScope = _logger.Scope(logger => logger.LogInformation($"Starting server '{GetType().Name}'"));
            var result = StopwatchHelper.Measure(CreateClient);
            loggerScope.Finish(logger => logger.LogInformation($"Server started ({result.ElapsedMilliseconds} ms)"));

            var httpClientAccessor = Services.GetRequiredService<LTestHttpClientAccessor>();
            httpClientAccessor.Client = result.ResultObject;
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
            // HttpClient
            services.AddSingleton<LTestHttpClientAccessor>();

            // Configuration
            services.AddSingleton(_configuration);

            // Logger
            services.AddSingleton(_logger);

            // LogSniffer
            services.AddSingleton<ILogSnifferService, LogSnifferService>();

            // DisposableCollector
            services.AddScoped<DisposableCollertor>();

            // Facade
            services.AddScoped<LTestFacade>();
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