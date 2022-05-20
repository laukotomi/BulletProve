using Example.Api.Controllers;
using FluentAssertions;
using LTest;
using LTest.Http.Services;
using System.Net;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace IntegrationTests.Controllers
{
    /// <summary>
    /// The auth controller tests.
    /// </summary>
    public class AuthControllerTests : LTestBase
    {
        private readonly HttpRequestBuilder<AuthController> _authController;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerTests"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        /// <param name="output">The output.</param>
        public AuthControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _authController = Services.GetHttpRequestBuilder<AuthController>();
        }

        /// <summary>
        /// Whens the credentials are ok then token returned.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreOk_ThenTokenReturned()
        {
            var token = await _authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword,
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            token.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Whens the credentials are bad then unauthorized result.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreBad_ThenUnauthorizedResult()
        {
            await _authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = "Badadd",
                })
                .Assert<string>()
                .AssertStatusCode(HttpStatusCode.Unauthorized)
                .ExecuteAsync();
        }

        /// <summary>
        /// Whens the user is not logged in then unauthorized returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsNotLoggedIn_ThenUnauthorizedReturned()
        {
            var userController = Services.GetHttpRequestBuilder<UserController>();

            await userController
                .CreateFor(x => nameof(x.GetUserDataAsync))
                .Assert<string>()
                .AssertStatusCode(HttpStatusCode.Unauthorized)
                .ExecuteAsync();
        }
    }
}
