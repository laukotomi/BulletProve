using LTest.ServerLog;
using LTest.Services;

namespace LTest.LogSniffer
{
    /// <summary>
    /// LogSniffer service.
    /// </summary>
    public interface IServerLogsService
    {
        /// <summary>
        /// Returns the actual snapshot of the events.
        /// </summary>
        IReadOnlyCollection<ServerLogEvent> GetServerLogs();

        /// <summary>
        /// Gets the expected logs.
        /// </summary>
        Inspector<ServerLogEvent> ServerLogInspector { get; }
    }
}