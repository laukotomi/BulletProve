using LTest.Logging;

namespace LTest.LogSniffer
{
    /// <summary>
    /// LogSniffer service.
    /// </summary>
    public interface ILogSnifferService
    {
        /// <summary>
        /// Check whether unexpected log event occured.
        /// </summary>
        bool UnexpectedLogOccured { get; }

        /// <summary>
        /// Returns the actual snapshot of the events.
        /// </summary>
        IReadOnlyCollection<ServerLogEvent> GetServerLogs();

        /// <summary>
        /// Saves log event into memory and checks wheter it was unexpected.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        bool CheckLogEvent(ServerLogEvent logEvent);

        /// <summary>
        /// Gets the expected logs.
        /// </summary>
        LogFilter<ServerLogEvent> ExpectedLogs { get; }
    }
}