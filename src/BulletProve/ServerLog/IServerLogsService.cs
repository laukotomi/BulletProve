using BulletProve.Services;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// LogSniffer service.
    /// </summary>
    public interface IServerLogsService
    {
        /// <summary>
        /// Returns the actual snapshot of the server log events.
        /// </summary>
        IReadOnlyCollection<ServerLogEvent> GetServerLogs();

        /// <summary>
        /// Gets the server log event inspector.
        /// </summary>
        Inspector<ServerLogEvent> ServerLogInspector { get; }
    }
}