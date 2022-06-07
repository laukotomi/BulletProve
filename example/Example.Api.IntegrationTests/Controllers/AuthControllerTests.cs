using Example.Api.Controllers;
using FluentAssertions;
using IntegrationTests.Common;
using LTest;
using LTest.Http.Services;
using System.Net;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Example.Api.IntegrationTests.Controllers
{
    /// <summary>
    /// The auth controller tests.
    /// </summary>
    public class AuthControllerTests : LTestBase
    {
        private readonly HttpRequestBuilder<AuthController> _authController;
        private readonly HttpRequestBuilder<UserController> _userController;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerTests"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        /// <param name="output">The output.</param>
        public AuthControllerTests(TestServerManager serverManager, ITestOutputHelper output)
            : base(serverManager, output)
        {
            _authController = LTestServices.GetHttpRequestBuilder<AuthController>();
            _userController = LTestServices.GetHttpRequestBuilder<UserController>();
        }

        /// <summary>
        /// Whens the credentials are ok then token returned.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreOk_ThenTokenReturned()
        {
            var token = await _authController
                .CreateFor(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword,
                })
                .ExecuteSuccessAsync<string>();

            token.Should().NotBeNullOrEmpty();

            using var response = await _userController
                .CreateRequest(x => x.GetUserDataAsync, token)
                .ExecuteSuccessAsync();
        }

        /// <summary>
        /// Whens the credentials are bad then unauthorized result.
        /// </summary>
        [Fact]
        public async Task WhenCredentialsAreBad_ThenUnauthorizedResult()
        {
            LTestServices.LogSniffer.ExpectedLogs.Add(x => x.Message == "Wrong username or password");

            using var response = await _authController
                .CreateFor(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = "Badadd",
                })
                .ExecuteAssertingStatusAsync(HttpStatusCode.Unauthorized);

            response.Should().NotBeNull();
        }

        /// <summary>
        /// Whens the user is not logged in then unauthorized returned.
        /// </summary>
        [Fact]
        public async Task WhenUserIsNotLoggedIn_ThenUnauthorizedReturned()
        {
            using var response = await _userController
                .CreateFor(x => x.GetUserDataAsync)
                .ExecuteAssertingStatusAsync(HttpStatusCode.Unauthorized);

            response.Should().NotBeNull();
        }
    }
}
