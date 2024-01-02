using Example.Api.Controllers;

namespace Example.Api.IntegrationTests.Controllers
{
    /// <summary>
    /// The user controller tests.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="UserControllerTests"/> class.
    /// </remarks>
    /// <param name="serverManager">The server manager.</param>
    /// <param name="output">The output.</param>
    public class UserControllerTests(ServerManager serverManager, ITestOutputHelper output)
        : TestBase(serverManager, output)
    {
        /// <summary>
        /// Whens the user is logged in then its data returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsLoggedIn_ThenItsDataReturned()
        {
            var token = await LoginAsAdminAndGetTokenAsync(Server);

            var userData = await Server
                .HttpRequestFor<UserController>(x => x.GetUserDataAsync)
                .WithToken(token)
                .ExecuteSuccessAsync<UserController.UserDto>();

            userData.Username.Should().Be(TestConstants.AdminUsername);
        }

        /// <summary>
        /// Whens the user is registered then it can login.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task WhenUserIsRegistered_ThenItCanLogin()
        {
            var adminToken = await LoginAsAdminAndGetTokenAsync(Server);
            var username = "NewUser";
            var password = "Password";

            using var response1 = await Server
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .AddAllowedServerLogEvent(x => x.Message == "Wrong username or password")
                .ExecuteAssertingStatusAsync(HttpStatusCode.Unauthorized);

            using var response2 = await Server
                .HttpRequestFor<UserController>(x => x.RegisterUserAsync)
                .WithToken(adminToken)
                .SetJsonContent(new UserController.RegisterUserCommand
                {
                    UserName = username,
                    Password = password
                })
                .ExecuteSuccessAsync();

            var token = await Server
                .HttpRequestFor<AuthController>(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .ExecuteSuccessAsync<string>();

            token.Should().NotBeNullOrEmpty();
        }
    }
}
