namespace LTest.Http
{
    /// <summary>
    /// Contains the <see cref="HttpClient"/> that can be used for integration tests.
    /// </summary>
    public class LTestHttpClientAccessor
    {
        /// <summary>
        /// <see cref="HttpClient"/> for integration tests.
        /// </summary>
        public HttpClient Client { get; set; } = null!;
    }
}