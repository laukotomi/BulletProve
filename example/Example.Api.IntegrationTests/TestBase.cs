using BulletProve;
using BulletProve.Hooks;
using BulletProve.TestServer;
using Example.Api.Controllers;
using Example.Api.Data;
using Example.Api.IntegrationTests.Hooks;
using Example.Api.IntegrationTests.Mocks;
using Example.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Example.Api.IntegrationTests
{
    public abstract class TestBase : TestClass, IAsyncLifetime
    {
        protected const string DefaultServer = "Default";

        protected ServerScope Server { get; private set; }

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
                    services.AddTestHttp();
                });
            });
        }

        public async Task InitializeAsync()
        {
            Server = await GetServerAsync(DefaultServer);
        }

        /// <summary>
        /// Logins the as admin and get token.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>A string.</returns>
        protected async Task<string> LoginAsAdminAndGetTokenAsync(ServerScope facade)
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

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
