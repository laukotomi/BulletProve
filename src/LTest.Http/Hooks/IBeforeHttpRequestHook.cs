using System.Net.Http;
using System.Threading.Tasks;

namespace LTest.Http.Interfaces
{
    /// <summary>
    /// A service that will be run before server Http request.
    /// </summary>
    public interface IBeforeHttpRequestHook
    {
        /// <summary>
        /// Will be run after server Http request was sent.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="httpRequest">The http request.</param>
        /// <returns>A Task.</returns>
        Task BeforeHttpRequestAsync(string label, HttpRequestMessage httpRequest);
    }
}