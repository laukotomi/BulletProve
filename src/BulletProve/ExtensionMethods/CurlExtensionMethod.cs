using BulletProve.Exceptions;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace System.Net.Http
{
    /// <summary>
    /// Curl extension method for <see cref="HttpRequestMessage"/>.
    /// </summary>
    public static class CurlExtensionMethod
    {
        /// <summary>
        /// Generates curl from http request message.
        /// </summary>
        /// <param name="request">The request.</param>
        public static async Task<string> ToCurlAsync(this HttpRequestMessage request)
        {
            var sb = new StringBuilder($"curl -X {request.Method.Method} '{request.RequestUri}'");

            AddHeaders(sb, request.Headers);

            if (request.Content != null)
            {
                AddHeaders(sb, request.Content.Headers);

                if (request.Content is StringContent)
                {
                    var data = await request.Content.ReadAsStringAsync();
                    sb.Append($" --data-raw '{data.Escape()}'");
                }
                else if (request.Content is FormUrlEncodedContent)
                {
                    var data = await request.Content.ReadAsStringAsync();
                    var collection = HttpUtility.ParseQueryString(data);

                    foreach (var key in collection.AllKeys)
                    {
                        var urlEncoded = HttpUtility.UrlEncode(collection[key]!).Replace('+', ' ');
                        sb.Append($" --data-urlencode '{key!.Escape()}={urlEncoded}'");
                    }
                }
                else
                {
                    throw new BulletProveException($"Unsupported content type {request.Content.GetType().Name}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds the headers.
        /// </summary>
        /// <param name="sb">The string builder.</param>
        /// <param name="headers">The headers.</param>
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

        /// <summary>
        /// Escapes the string.
        /// </summary>
        /// <param name="str">The str.</param>
        /// <returns>A string.</returns>
        private static string Escape(this string str) => str.Replace("'", "'\\''");
    }
}