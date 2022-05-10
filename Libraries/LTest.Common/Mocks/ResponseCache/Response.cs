namespace LTest.Mocks.ResponseCache
{
    /// <summary>
    /// Response object.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Status code.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Content.
        /// </summary>
        public Content Content { get; set; }

        /// <summary>
        /// Headers.
        /// </summary>
        public string Headers { get; set; }

        /// <summary>
        /// Trailing headers.
        /// </summary>
        public string TrailingHeaders { get; set; }

        /// <summary>
        /// Reason phrase.
        /// </summary>
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// Serialized version.
        /// </summary>
        public string Version { get; set; }
    }
}