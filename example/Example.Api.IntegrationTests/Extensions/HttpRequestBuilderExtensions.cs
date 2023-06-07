using LTest.Http.Services;
using Microsoft.Net.Http.Headers;

namespace Example.Api.IntegrationTests.Extensions
{
    /// <summary>
    /// The http request builder extensions.
    /// </summary>
    public static class HttpRequestBuilderExtensions
    {
        /// <summary>
        /// Adds authorization token to the request.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="token">The token.</param>
        /// <returns>A HttpRequestBuilder.</returns>
        public static HttpRequestBuilder WithToken(this HttpRequestBuilder builder, string token)
        {
            return builder.SetHeaders(x => x.TryAddWithoutValidation(HeaderNames.Authorization, token));
        }
    }
}
