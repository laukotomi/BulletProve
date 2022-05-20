namespace LTest.Http.Configuration
{
    /// <summary>
    /// Configuration.
    /// </summary>
    public class HttpConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpConfiguration"/> class.
        /// </summary>
        /// <param name="httpHeaderLogMaxLength">The maximum length of a http header content to log.</param>
        /// <param name="httpContentLogMaxLength">The maximum length of a http message content to log.</param>
        public HttpConfiguration(int httpHeaderLogMaxLength, int httpContentLogMaxLength)
        {
            HttpHeaderLogMaxLength = httpHeaderLogMaxLength;
            HttpContentLogMaxLength = httpContentLogMaxLength;
        }

        /// <summary>
        /// The maximum length of a http header content to log.
        /// </summary>
        public int HttpHeaderLogMaxLength { get; }

        /// <summary>
        /// The maximum length of a http message content to log.
        /// </summary>
        public int HttpContentLogMaxLength { get; }
    }
}