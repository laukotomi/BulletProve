using LTest.Hooks;
using LTest.Http.Models;

namespace LTest.Http.Interfaces
{
    /// <summary>
    /// A service that will be run before server Http request.
    /// </summary>
    public interface IBeforeHttpRequestHook : IHook
    {
        /// <summary>
        /// Will be run after server Http request was sent.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="httpRequest">The http request.</param>
        /// <returns>A Task.</returns>
        Task BeforeHttpRequestAsync(HttpRequestContext context);
    }
}