using LTest.Behaviors;
using LTest.Configuration;
using LTest.Helpers;
using LTest.Http;
using LTest.Interfaces;
using LTest.Logger;
using LTest.LogSniffer;
using LTest.Mocks;
using LTest.Mocks.ResponseCache;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace LTest
{
    /// <summary>
    /// WebApplication factory base class for integration tests.
    /// </summary>
    /// <typeparam name="TStartup">Startup class.</typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class TestServerBase<TStartup> : WebApplicationFactory<TStartup>, ITestServer
        where TStartup : class
    {
        private bool _started;
        private IntegrationTestConfiguration _configuration;
        private ITestLogger _logger;

        /// <summary>
        /// Ensures that the server is running.
        /// </summary>
        public bool EnsureServerStarted()
        {
            if (_started)
            {
                return false;
            }

            _started = true;
            _logger = new TestLogger();
            _configuration = InitConfiguration();

            using var loggerScope = _logger.Scope(logger => logger.Info("Starting server"));
            var client = StopwatchHelper.MeasureMilliseconds(() => CreateClient(), out var serverStartTime);
            loggerScope.Finish(logger => logger.Info($"Server {GetType().Name} started ({serverStartTime} ms)"));

            var httpClientAccessor = Services.GetRequiredService<HttpClientAccessor>();
            httpClientAccessor.Client = client;

            ServicesHelper.RunServices<IAfterServerStartedBehavior>(Services);
            return true;
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
        protected abstract void Configure(IntegrationTestConfiguration config);

        /// <summary>
        /// ConfigureWebHost.
        /// </summary>
        /// <param name="builder">IWebHostBuilder.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                RegisterCommonServices(services);
                ConfigureTestServices(services);
                RegisterCleanableServices(services);
            });
        }

        /// <summary>
        /// Creates the <see cref="IHostBuilder"/>.
        /// </summary>
        protected override IHostBuilder CreateHostBuilder()
        {
            return base.CreateHostBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseSetting("https_port", "443");
                })
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
                        builder.Services.AddSingleton<ILoggerFactory, LogSnifferLoggerFactory>();
                    }
                    else
                    {
                        _logger.Warning("Default LogSniffer registration was skipped.");
                    }
                });
        }

        private void RegisterCommonServices(IServiceCollection services)
        {
            // HttpClient
            services.AddSingleton<HttpClientAccessor>();

            // Configuration
            services.AddSingleton(_configuration);

            // Logger
            services.AddSingleton(_logger);

            // LogSniffer
            services.AddSingleton<ILogSnifferService, LogSnifferService>();
            services.AddSingleton<CategoryNameCollector>();

            // Behaviors
            services.AddSingleton<IBeforeTestBehavior, CleanSingletonsBeforeTestBehavior>();

            // Mocks
            services.AddSingleton(typeof(DefaultMessageHandler<>));
            services.AddSingleton(typeof(ResponseCacheService<>));
        }

        private void RegisterCleanableServices(IServiceCollection services)
        {
            var type = typeof(ICleanSingletonBeforeTest);
            var cleanableServices = services
                .Where(x => x.ImplementationType != null && x.ImplementationType.IsAssignableTo(type))
                .ToList();

            foreach (var service in cleanableServices)
            {
                if (service.Lifetime != ServiceLifetime.Singleton)
                {
                    throw new InvalidOperationException($"{nameof(ICleanSingletonBeforeTest)} can be used only with singletons");
                }

                services.AddSingleton(sp =>
                {
                    if (service.ServiceType != null)
                    {
                        return sp.GetRequiredService(service.ServiceType) as ICleanSingletonBeforeTest;
                    }
                    return sp.GetRequiredService(service.ImplementationType) as ICleanSingletonBeforeTest;
                });
            }
        }

        private IntegrationTestConfiguration InitConfiguration()
        {
            var config = new IntegrationTestConfiguration();
            Configure(config);
            return config;
        }
    }
}