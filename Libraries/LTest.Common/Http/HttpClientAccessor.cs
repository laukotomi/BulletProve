using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace LTest.Http
{
    /// <summary>
    /// Contains the <see cref="HttpClient"/> that can be used for integration tests.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class HttpClientAccessor
    {
        /// <summary>
        /// <see cref="HttpClient"/> for integration tests.
        /// </summary>
        public HttpClient Client { get; set; }
    }
}