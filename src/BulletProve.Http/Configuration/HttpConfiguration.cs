using System.Text.Json;

namespace BulletProve.Http.Configuration
{
    /// <summary>
    /// Configuration.
    /// </summary>
    public class HttpConfiguration
    {
        /// <summary>
        /// The maximum length of a http header content to log. Default is 50.
        /// </summary>
        public int HttpHeaderLogMaxLength { get; set; } = 50;

        /// <summary>
        /// The maximum length of a http message content to log. Default is 1000.
        /// </summary>
        public int HttpContentLogMaxLength { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the json serializer options.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }
}