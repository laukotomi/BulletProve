namespace LTest.Http.Configuration
{
    /// <summary>
    /// Configuration.
    /// </summary>
    public class HttpConfiguration
    {
        private static readonly int _httpHeaderLogMaxLength = 50;
        private static readonly int _httpContentLogMaxLength = 1000;

        /// <summary>
        /// The maximum length of a http header content to log. Default is <see cref="_httpHeaderLogMaxLength"/>.
        /// </summary>
        public int HttpHeaderLogMaxLength { get; } = _httpHeaderLogMaxLength;

        /// <summary>
        /// The maximum length of a http message content to log. Default is <see cref="_httpContentLogMaxLength"/>.
        /// </summary>
        public int HttpContentLogMaxLength { get; } = _httpContentLogMaxLength;
    }
}