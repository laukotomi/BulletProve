using Example.Api.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Api.IntegrationTests.Mocks
{
    /// <summary>
    /// The external service mock.
    /// </summary>
    public class ExternalServiceMock : IExternalService
    {
        /// <summary>
        /// Checks whether the user is correct.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}
