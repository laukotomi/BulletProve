using BulletProve.Http.Services;
using System.Text.Json;

namespace BulletProve.Http.Configuration
{
    /// <summary>
    /// Configuration.
    /// </summary>
    public class HttpConfiguration
    {
        public HttpConfiguration()
        {
            JsonSerializerOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            ResponseMessageDeserializer = new JsonResponseMessageDeserializer(JsonSerializerOptions);
        }

        /// <summary>
        /// The maximum length of a http message content to log. Default is 1000.
        /// </summary>
        public int HttpContentLogMaxLength { get; set; } = 1000;

        /// <summary>
        /// The maximum length of a http header content to log. Default is 50.
        /// </summary>
        public int HttpHeaderLogMaxLength { get; set; } = 50;

        /// <summary>
        /// Gets the response message deserializer.
        /// </summary>
        public ResponseMessageDeserializer ResponseMessageDeserializer { get; }

        /// <summary>
        /// Gets the json serializer options.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; }
    }
}