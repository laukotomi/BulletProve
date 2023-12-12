using BulletProve.ServerLog;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The assertion runner.
    /// </summary>
    public interface IAssertionRunner<TResponse>
        where TResponse : class
    {
        /// <summary>
        /// Runs the response message assertions.
        /// </summary>
        /// <param name="response"></param>
        void RunResponseMessageAssertions(HttpResponseMessage response);

        /// <summary>
        /// Runs the server log assertions.
        /// </summary>
        /// <param name="serverLogs">The server logs.</param>
        void RunResponseObjectAssertions(TResponse responseObject);

        /// <summary>
        /// Runs the response object assertions.
        /// </summary>
        /// <param name="responseObject">The response object.</param>
        void RunServerLogAssertions(IReadOnlyCollection<ServerLogEvent> serverLogs);
    }
}
