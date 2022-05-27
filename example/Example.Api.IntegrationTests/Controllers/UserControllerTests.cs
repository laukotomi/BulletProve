using Example.Api.Controllers;
using FluentAssertions;
using IntegrationTests.Common;
using LTest;
using LTest.Http.Services;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Example.Api.IntegrationTests.Controllers
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
            _userController = LTestServices.GetHttpRequestBuilder<UserController>();
        }

        /// <summary>
        /// Whens the user is logged in then its data returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsLoggedIn_ThenItsDataReturned()
        {
            var token = await _userController.LoginAsAdminAndGetTokenAsync();

            var userData = await _userController
                .CreateFor(x => x.GetUserDataAsync)
                .SetHeaders(x => x.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token))
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
            var authController = LTestServices.GetHttpRequestBuilder<AuthController>();
            var adminToken = await _userController.LoginAsAdminAndGetTokenAsync();
            var username = "NewUser";
            var password = "Password";

            using (LTestServices.LogSniffer.ExpectedLogs.Add(x => x.Message == "Wrong username or password"))
            {
                using var response1 = await authController
                    .CreateFor(x => x.LoginAsync)
                    .SetJsonContent(new AuthController.LoginCommand
                    {
                        Username = username,
                        Password = password
                    })
                    .ExecuteAssertingStatusAsync(System.Net.HttpStatusCode.Unauthorized);
            }

            using var response2 = await _userController
                .CreateRequest(x => x.RegisterUserAsync, adminToken)
                .SetJsonContent(new UserController.RegisterUserCommand
                {
                    UserName = username,
                    Password = password
                })
                .ExecuteSuccessAsync();

            var token = await authController
                .CreateFor(x => x.LoginAsync)
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
