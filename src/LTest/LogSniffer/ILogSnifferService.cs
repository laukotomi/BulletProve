using LTest.Logging;
using LTest.Services;

namespace LTest.LogSniffer
{
    /// <summary>
    /// LogSniffer service.
    /// </summary>
    public interface ILogSnifferService
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