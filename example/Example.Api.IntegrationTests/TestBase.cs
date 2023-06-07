using Example.Api.Controllers;
using LTest;
using LTest.TestServer;
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

        public override void RegisterServers(ServerRegistrator serverRegistrator)
        {
            serverRegistrator.RegisterServer<TestServer>(DefaultServer);
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
