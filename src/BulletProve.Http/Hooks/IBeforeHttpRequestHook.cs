using BulletProve.Base.Hooks;
using BulletProve.Http.Models;

namespace BulletProve.Http.Interfaces
{
    /// <summary>
    /// A service that will be run before server Http request.
    /// </summary>
    public interface IBeforeHttpRequestHook : IHook
    {
        /// <summary>
        /// Will be run after server Http request was sent.
        /// </summary>
        /// <param name="context">Http request context.</param>
        /// <returns>A Task.</returns>
        Task BeforeHttpRequestAsync(HttpRequestContext context);
    }
}