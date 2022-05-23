using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace LTest.Mocks
{
    /// <summary>
    /// The LTest http message handler base.
    /// </summary>
    public abstract class LTestHttpMessageHandlerBase : HttpMessageHandler
    {
        /// <summary>
        /// Creates the json response message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="content">The content.</param>
        /// <returns>A HttpResponseMessage.</returns>
        protected static HttpResponseMessage CreateJsonResponseMessage(HttpRequestMessage request, HttpStatusCode statusCode, object content)
        {
            var json = JsonSerializer.Serialize(content);

            return new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json),
                RequestMessage = request
            };
        }
    }
}
