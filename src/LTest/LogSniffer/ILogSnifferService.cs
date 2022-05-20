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
        void CheckLogEvent(ServerLogEvent logEvent);

        /// <summary>
        /// Modifies expected log filters.
        /// </summary>
        /// <param name="action">Action.</param>
        ResetExpectedServerLogs ModifyExpectedLogEvents(Action<LogFilter<ServerLogEvent>> action);
    }
}