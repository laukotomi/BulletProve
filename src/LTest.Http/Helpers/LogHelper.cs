using LTest.ExtensionMethods;
using LTest.Http.Configuration;
using System.Text;

namespace LTest.Http.Helpers
{
    /// <summary>
    /// Helper class for logs.
    /// </summary>
    internal static class LogHelper
    {
        /// <summary>
        /// Creates log for http request.
        /// </summary>
        /// <param name="request">Http request.</param>
        /// <param name="client">Http client.</param>
        /// <param name="configuration">Configuration.</param>
        public static string CreateRequestLog(HttpRequestMessage request, HttpClient client, HttpConfiguration configuration)
        {
            var builder = new StringBuilder($"{request.Method} {request.RequestUri}");
            var headers = client.DefaultRequestHeaders.Concat(request.Headers);

            LogHeaders(builder, headers, configuration);
            if (request.Content != null)
            {
                LogContent(builder, request.Content, configuration);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Creates log for http response.
        /// </summary>
        /// <param name="response">Http response.</param>
        /// <param name="elapsedMilliseconds">Elapsed milliseconds.</param>
        /// <param name="configuration">Configuration.</param>
        public static string CreateResponseLog(HttpResponseMessage response, long elapsedMilliseconds, HttpConfiguration configuration)
        {
            var builder = new StringBuilder($"{(int)response.StatusCode} ({response.ReasonPhrase}), {elapsedMilliseconds} ms");

            LogHeaders(builder, response.Headers, configuration);
            LogContent(builder, response.Content, configuration);

            return builder.ToString();
        }

        /// <summary>
        /// Logs the content.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="content">The content.</param>
        /// <param name="configuration">The configuration.</param>
        private static void LogContent(StringBuilder builder, HttpContent content, HttpConfiguration configuration)
        {
            if (content.Headers.ContentType != null && IsLoggableMediaType(content.Headers.ContentType?.MediaType))
            {
                var responseMessage = content.ReadAsStringAsync().GetAwaiter().GetResult();

                if (!string.IsNullOrWhiteSpace(responseMessage))
                {
                    builder.Append($", Content: {responseMessage.Truncate(configuration.HttpContentLogMaxLength)}");
                }
            }
        }

        /// <summary>
        /// Determines whether the media type is loggable.
        /// </summary>
        /// <param name="mediaType">The media type.</param>
        /// <returns>A bool.</returns>
        private static bool IsLoggableMediaType(string? mediaType)
        {
            if (string.IsNullOrEmpty(mediaType))
            {
                return false;
            }

            return mediaType.StartsWith("text", StringComparison.OrdinalIgnoreCase)
                || mediaType.EndsWith("json", StringComparison.OrdinalIgnoreCase)
                || mediaType.EndsWith("xml", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Logs the headers.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="configuration">The configuration.</param>
        private static void LogHeaders(StringBuilder builder, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, HttpConfiguration configuration)
        {
            if (!headers.Any())
            {
                return;
            }

            var headerTexts = headers.Select(x =>
            {
                var value = string.Join("; ", x.Value).Truncate(configuration.HttpHeaderLogMaxLength);

                return $"{x.Key}: {value}";
            });

            builder.Append($", Headers: {{ {string.Join(", ", headerTexts)} }}");
        }
    }
}