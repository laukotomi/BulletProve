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
using Xunit.Abstractions;

namespace Example.Api.IntegrationTests
{
    /// <summary>
    /// The test base class.
    /// </summary>
    public abstract class TestBase : TestClass
    {
        /// <summary>
        /// The default server name.
        /// </summary>
        protected const string DefaultServer = "Default";

        /// <summary>
        /// Gets the default server.
        /// </summary>
        protected ServerScope Server { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        /// <param name="output">The output.</param>
        protected TestBase(TestServerManager serverManager, ITestOutputHelper output) : base(serverManager, output)
        {
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Server = await GetServerAsync(DefaultServer);
        }

        /// <summary>
        /// Logins the as admin and get token.
        /// </summary>
        /// <param name="scope">Server scope.</param>
        protected async Task<string> LoginAsAdminAndGetTokenAsync(ServerScope scope)
        {
            var token = await scope
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetLabel("AdminLogin")
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword
                })
                .ExecuteSuccessAsync<string>();

            return token;
        }
    }
}
