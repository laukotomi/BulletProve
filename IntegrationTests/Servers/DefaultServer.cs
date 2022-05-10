using IntegrationTests.Behaviors;
using IntegrationTests.Mocks;
using IntegTest;
using IntegTest.Data;
using IntegTest.Services;
using LTest;
using LTest.Attributes;
using LTest.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Servers
{
    [DefaultTestServer]
    public class DefaultServer : TestServerBase<Startup>
    {
        protected override void Configure(IntegrationTestConfiguration config)
        {
        }

        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // Behaviors
            services.AddCleanDatabaseBehavior<AppDbContext>();
            services.AddSingleton<IBeforeTestBehavior, SeedDatabaseBehavior>();

            // Mocks
            services.AddTransient<IExternalService, ExternalServiceMock>();

            // Packages
            services.AddTestHttp();
            services.AddTestEFCore();
        }
    }
}
