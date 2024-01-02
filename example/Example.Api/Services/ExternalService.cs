using Microsoft.Extensions.Configuration;
using System.Net;
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
        private readonly string _url;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalService"/> class.
        /// </summary>
        /// <param name="httpClient">The http client.</param>
        /// <param name="configuration">The configuration object.</param>
        public ExternalService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _url = configuration.GetValue<string>("ExternalServiceUrl")!;
        }

        /// <summary>
        /// Are the user correct async.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        public async Task<bool> IsUserCorrectAsync(string username, CancellationToken cancellationToken)
        {
            var result = await _httpClient.GetAsync(_url, cancellationToken);

            return result.StatusCode == HttpStatusCode.OK;
        }
    }
}
