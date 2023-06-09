using Example.Api.Controllers;
using Example.Api.Data;
using Example.Api.IntegrationTests.Hooks;
using Example.Api.IntegrationTests.Mocks;
using Example.Api.Services;
using LTest;
using LTest.Hooks;
using LTest.TestServer;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Example.Api.IntegrationTests
{
    public abstract class TestBase : LTestBase
    {
        protected const string DefaultServer = "Default";

        protected LTestFacade Server { get; private set; }

        protected TestBase(TestServerManager serverManager, ITestOutputHelper output) : base(serverManager, output)
        {
        }

        public override void RegisterServers(IServerRegistrator serverRegistrator)
        {
            serverRegistrator.RegisterServer<Startup>(DefaultServer, config =>
            {
                config.ConfigureTestServices(services =>
                {
                    // Hooks
                    services.AddCleanDatabaseHook<AppDbContext>();
                    services.AddScoped<IBeforeTestHook, SeedDatabaseHook>();

                    // Mocks
                    services.AddTransient<IExternalService, ExternalServiceMock>();

                    // Packages
                    services.AddLTestHttp();
                    services.AddLTestEFCore();
                });
            });
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Server = await GetServerAsync(DefaultServer);
        }

        /// <summary>
        /// Logins the as admin and get token.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>A string.</returns>
        protected async Task<string> LoginAsAdminAndGetTokenAsync(LTestFacade facade)
        {
            var token = await facade
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetLabel("AdminLogin")
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            return token;
        }
    }
}
