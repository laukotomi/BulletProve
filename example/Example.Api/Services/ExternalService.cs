using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Api.Services
{
    /// <summary>
    /// The external service.
    /// </summary>
    public interface IExternalService
    {
        /// <summary>
        /// Are the user correct async.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken);
    }

    /// <summary>
    /// The external service.
    /// </summary>
    public class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalService"/> class.
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        public ExternalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Are the user correct async.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public async Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken)
        {
            var result = await _httpClient.GetAsync("https://google.com", cancellationToken);
            if (result != null)
            {
                return true;
            }

            return false;
        }
    }
}
