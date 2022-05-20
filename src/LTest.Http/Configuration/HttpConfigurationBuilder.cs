namespace LTest.Http.Configuration
{
    /// <summary>
    /// Configuration builder.
    /// </summary>
    public class HttpConfigurationBuilder
    {
        private int _httpHeaderLogMaxLength = 50;
        private int _httpContentLogMaxLength = 1000;

        /// <summary>
        /// Sets the max length of the http header log. Default is 50.
        /// </summary>
        /// <param name="value">The new value.</param>
        public HttpConfigurationBuilder SetHttpHeaderLogMaxLength(int value)
        {
            _httpHeaderLogMaxLength = value;
            return this;
        }

        /// <summary>
        /// Sets the max length of the request / response content log. Default is 1000.
        /// </summary>
        /// <param name="value">The new value.</param>
        public HttpConfigurationBuilder SetHttpContentLogMaxLength(int value)
        {
            _httpContentLogMaxLength = value;
            return this;
        }

        /// <summary>
        /// Returns a new <see cref="HttpConfiguration"/> instance.
        /// </summary>
        public HttpConfiguration Build()
        {
            return new HttpConfiguration(_httpHeaderLogMaxLength, _httpContentLogMaxLength);
        }
    }
}