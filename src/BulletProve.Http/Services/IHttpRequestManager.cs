using BulletProve.Http.Models;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The http request manager.
    /// </summary>
    public interface IHttpRequestManager
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scope">The server scope.</param>
        Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestContext context, IServerScope scope);
    }
}
