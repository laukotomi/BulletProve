using LTest.Logging;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Helper class to reset LogSniffer expected log event action.
    /// </summary>
    public sealed class ResetExpectedServerLogs : IDisposable
    {
        private readonly LogFilter<ServerLogEvent> _logEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetExpectedServerLogs"/> class.
        /// </summary>
        /// <param name="logEventStore">The log event store.</param>
        public ResetExpectedServerLogs(LogFilter<ServerLogEvent> logEventStore)
        {
            _logEventStore = logEventStore;
        }

        /// <summary>
        /// Resets log event store.
        /// </summary>
        public void Dispose()
        {
            _logEventStore.Reset();
        }
    }
}