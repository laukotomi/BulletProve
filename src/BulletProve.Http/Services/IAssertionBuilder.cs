using BulletProve.ServerLog;
using System.Net;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The assertion builder.
    /// </summary>
    public interface IAssertionBuilder<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Runs assert logic on the response message.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        IAssertionBuilder<TResponse> AssertResponseMessage(Action<HttpResponseMessage> assertAction);

        /// <summary>
        /// Runs assert logic on the response.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        IAssertionBuilder<TResponse> AssertResponseObject(Action<TResponse> assertAction);

        /// <summary>
        /// Runs assert logic on server log events.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        IAssertionBuilder<TResponse> AssertServerLogs(Action<IReadOnlyCollection<ServerLogEvent>> assertAction);

        /// <summary>
        /// Runs assert login on response statuscode.
        /// </summary>
        /// <param name="statusCode">Expected status code.</param>
        IAssertionBuilder<TResponse> AssertStatusCode(HttpStatusCode statusCode);

        /// <summary>
        /// Builds the assertion runner.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>An AssertionRunner.</returns>
        IAssertionRunner<TResponse> BuildAssertionRunner(IServerScope scope);

        /// <summary>
        /// Ensures that the status code of the response is in [200-299].
        /// </summary>
        IAssertionBuilder<TResponse> EnsureSuccessStatusCode();
    }
}