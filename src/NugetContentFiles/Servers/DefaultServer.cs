using Example.Api;
using Example.Api.Data;
using Example.Api.IntegrationTests.Hooks;
using Example.Api.Services;
using IntegrationTests.Mocks;
using LTest.Configuration;
using LTest.Hooks;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
{
    /// <summary>
    /// The default server.
    /// </summary>
    [DefaultTestServer]
    public class DefaultServer : TestServerBase<Startup>
    {
        /// <inheritdoc/>
        protected override void Configure(LTestConfiguration config)
        {
        }

        /// <inheritdoc/>
        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // Hooks

            // Mocks

            // Packages
            //services.AddTestHttp();
        }
    }
}
