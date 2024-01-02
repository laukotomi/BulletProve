using Example.Api.Controllers;
using Example.Api.Data;
using Example.Api.IntegrationTests.Hooks;
using Example.Api.IntegrationTests.Mocks;
using Example.Api.Services;
using Microsoft.Extensions.DependencyInjection;

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
        protected IServerScope Server { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        /// <param name="output">The output.</param>
        protected TestBase(ServerManager serverManager, ITestOutputHelper output) : base(serverManager, output)
        {
        }

        /// <inheritdoc />
        protected override void RegisterServers(IServerRegistrator serverRegistrator)
        {
            serverRegistrator.RegisterServer<Startup>(DefaultServer, config =>
            {
                config.AddBulletProveHttp();

                config.AddBulletProveEfCore(efCore => efCore.CleanDatabase<AppDbContext, SeedDatabase>());

                config.ConfigureLogger(logger =>
                {
                    logger.ConfigureAllowedLoggerCategoryNames(inspector =>
                    {
                        inspector.AddDefaultAllowedAction(x => x.StartsWith("Microsoft.EntityFrameworkCore"), "EfCore");
                    });
                });

                config.ConfigureTestServices(services =>
                {
                    services.AddTransient<IExternalService, ExternalServiceMock>();
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
        protected async Task<string> LoginAsAdminAndGetTokenAsync(IServerScope scope)
        {
            var token = await scope
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetLabel("AdminLogin")
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword
                })
                .ExecuteSuccessAsync<string>(assert =>
                {
                    assert.AssertResponseObject(token => token.Should().NotBeNullOrEmpty());
                });

            return token;
        }
    }
}
