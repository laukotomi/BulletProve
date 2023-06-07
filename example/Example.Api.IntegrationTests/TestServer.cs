using Example.Api.Data;
using Example.Api.IntegrationTests.Hooks;
using Example.Api.IntegrationTests.Mocks;
using Example.Api.Services;
using LTest;
using LTest.Configuration;
using LTest.Hooks;
using Microsoft.Extensions.DependencyInjection;

namespace Example.Api.IntegrationTests
{
    /// <summary>
    /// The default server.
    /// </summary>
    [DefaultTestServer]
    public class TestServer : TestServerBase<Startup>
    {
        /// <inheritdoc/>
        protected override void Configure(LTestConfiguration config)
        {
            config.MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information;
        }

        /// <inheritdoc/>
        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // Hooks
            services.AddCleanDatabaseHook<AppDbContext>();
            services.AddScoped<IBeforeTestHook, SeedDatabaseHook>();

            // Mocks
            services.AddTransient<IExternalService, ExternalServiceMock>();

            // Packages
            services.AddLTestHttp();
            services.AddLTestEFCore();
        }
    }
}
