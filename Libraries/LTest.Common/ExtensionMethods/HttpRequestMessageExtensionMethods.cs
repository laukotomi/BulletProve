using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace System.Net.Http
{
    /// <summary>
    /// Http request message extensions.
    /// </summary>
    public static class HttpRequestMessageExtensionMethods
    {
        /// <summary>
        /// Generates curl from http request message.
        /// </summary>
        /// <param name="request">Request.</param>
        public static string ToCurl(this HttpRequestMessage request)
        {
            var sb = new StringBuilder($"curl -X {request.Method.Method} '{request.RequestUri}'");

            AddHeaders(sb, request.Headers);

            if (request.Content != null)
            {
                AddHeaders(sb, request.Content.Headers);

                if (request.Content is StringContent)
                {
                    var data = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    sb.Append($" --data-raw '{data.Escape()}'");
                }
                else if (request.Content is FormUrlEncodedContent)
                {
                    var data = request.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var collection = HttpUtility.ParseQueryString(data);

                    foreach (var key in collection.AllKeys)
                    {
                        var urlEncoded = HttpUtility.UrlEncode(collection[key]).Replace('+', ' ');
                        sb.Append($" --data-urlencode '{key.Escape()}={urlEncoded}'");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unsupported content type");
                }
            }

            return sb.ToString();
        }

        private static void AddHeaders(StringBuilder sb, HttpHeaders headers)
        {
            foreach ((var key, var values) in headers)
            {
                if (key.Equals(HeaderNames.ContentLength, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var value in values)
                {
                    sb.Append($" -H '{key.Escape()}: {value.Escape()}'");
                }
            }
        }

        private static string Escape(this string str)
        {
            return str
                .Replace("'", "'\\''");
        }
    }
}