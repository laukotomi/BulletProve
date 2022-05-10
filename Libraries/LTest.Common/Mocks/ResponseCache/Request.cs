namespace LTest.Mocks.ResponseCache
{
    /// <summary>
    /// Request object.
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Method.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Uri.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// Content.
        /// </summary>
        public Content Content { get; set; }

        /// <summary>
        /// Headers.
        /// </summary>
        public string Headers { get; set; }
    }
}