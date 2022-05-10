using FluentAssertions;
using IntegrationTests.Common;
using IntegTest.Controllers;
using LTest;
using LTest.Http.Services;
using Xunit.Abstractions;

namespace IntegrationTests.Controllers
{
    public class UserControllerTests : IntegrationTestBase
    {
        private readonly HttpRequestBuilder<UserController> _userController;

        public UserControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _userController = Services.GetHttpRequestBuilder<UserController>();
        }

        [Fact]
        public void WhenUserIsLoggedIn_ThenItsDataReturned()
        {
            var token = _userController.LoginAsAdminAndGetToken(Services);

            var userData = _userController
                .CreateFor(x => nameof(x.GetUserDataAsync))
                .SetHeaders(x => x.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(token))
                .Assert<UserController.UserDto>()
                .EnsureSuccessStatusCode()
                .Execute();

            userData.Username.Should().Be(TestConstants.AdminUsername);
        }

        [Fact]
        public void WhenUserIsRegistered_ThenItCanLogin()
        {
            var authController = Services.GetHttpRequestBuilder<AuthController>();
            var adminToken = _userController.LoginAsAdminAndGetToken(Services);
            var username = "NewUser";
            var password = "Password";

            authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .Assert<string>()
                .AssertStatusCode(System.Net.HttpStatusCode.Unauthorized)
                .Execute();

            _userController
                .CreateRequest(x => nameof(x.RegisterUserAsync), adminToken)
                .SetJsonContent(new UserController.RegisterUserCommand
                {
                    UserName = username,
                    Password = password
                })
                .Assert()
                .EnsureSuccessStatusCode()
                .Execute();

            var token = authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = username,
                    Password = password
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .Execute();

            token.Should().NotBeNullOrEmpty();
        }
    }
}
