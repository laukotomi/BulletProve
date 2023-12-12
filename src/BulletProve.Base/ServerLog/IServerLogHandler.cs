using BulletProve.Services;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// The server log inspector.
    /// </summary>
    public interface IServerLogHandler : IInspector<ServerLogEvent>
    {
        /// <summary>
        /// Handles the server log event.
        /// </summary>
        /// <param name="serverLogEvent">The log event.</param>
        void HandleServerLog(ServerLogEvent serverLogEvent);
    }
}
