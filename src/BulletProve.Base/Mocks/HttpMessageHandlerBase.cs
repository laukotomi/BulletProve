using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace BulletProve.Mocks
{
    /// <summary>
    /// The http message handler base.
    /// </summary>
    public abstract class HttpMessageHandlerBase : HttpMessageHandler
    {
        /// <summary>
        /// Creates the json response message.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="content">The content.</param>
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
