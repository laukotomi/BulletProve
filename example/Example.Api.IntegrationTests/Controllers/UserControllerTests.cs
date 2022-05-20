using Example.Api.Controllers;
using FluentAssertions;
using IntegrationTests.Common;
using LTest;
using LTest.Http.Services;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace IntegrationTests.Controllers
{
    /// <summary>
    /// The user controller tests.
    /// </summary>
    public class UserControllerTests : LTestBase
    {
        private readonly HttpRequestBuilder<UserController> _userController;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserControllerTests"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        /// <param name="output">The output.</param>
        public UserControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _userController = Services.GetHttpRequestBuilder<UserController>();
        }

        /// <summary>
        /// Whens the user is logged in then its data returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsLoggedIn_ThenItsDataReturned()
        {
            var token = await _userController.LoginAsAdminAndGetTokenAsync(Services);

            var userData = await _userController
                .CreateFor(x => nameof(x.GetUserDataAsync))
                .SetHeaders(x => x.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token))
                .Assert<UserController.UserDto>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            userData.Username.Should().Be(TestConstants.AdminUsername);
        }

        /// <summary>
        /// Whens the user is registered then it can login.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task WhenUserIsRegistered_ThenItCanLogin()
        {
            var authController = Services.GetHttpRequestBuilder<AuthController>();
            var adminToken = await _userController.LoginAsAdminAndGetTokenAsync(Services);
            var username = "NewUser";
            var password = "Password";

            await authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .Assert<string>()
                .AssertStatusCode(System.Net.HttpStatusCode.Unauthorized)
                .ExecuteAsync();

            await _userController
                .CreateRequest(x => nameof(x.RegisterUserAsync), adminToken)
                .SetJsonContent(new UserController.RegisterUserCommand
                {
                    UserName = username,
                    Password = password
                })
                .Assert()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            var token = await authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            token.Should().NotBeNullOrEmpty();
        }
    }
}
