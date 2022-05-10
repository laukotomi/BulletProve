using FluentAssertions;
using IntegTest.Controllers;
using LTest;
using LTest.Http.Services;
using System.Net;
using Xunit.Abstractions;

namespace IntegrationTests.Controllers
{
    public class AuthControllerTests : IntegrationTestBase
    {
        private readonly HttpRequestBuilder<AuthController> _authController;

        public AuthControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _authController = Services.GetHttpRequestBuilder<AuthController>();
        }

        [Fact]
        public void WhenCredentialsAreOk_ThenTokenReturned()
        {
            var token = _authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword,
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .Execute();

            token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void WhenCredentialsAreBad_ThenUnauthorizedResult()
        {
            _authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = "Badadd",
                })
                .Assert<string>()
                .AssertStatusCode(System.Net.HttpStatusCode.Unauthorized)
                .Execute();
        }

        [Fact]
        public void WhenUserIsNotLoggedIn_ThenUnauthorizedReturned()
        {
            var userController = Services.GetHttpRequestBuilder<UserController>();

            userController
                .CreateFor(x => nameof(x.GetUserDataAsync))
                .Assert<string>()
                .AssertStatusCode(HttpStatusCode.Unauthorized)
                .Execute();
        }
    }
}
