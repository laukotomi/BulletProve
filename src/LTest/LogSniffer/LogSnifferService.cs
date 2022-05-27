using LTest.Configuration;
using LTest.Hooks;
using LTest.Logging;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Log sniffer service implementation.
    /// </summary>
    public class LogSnifferService : ILogSnifferService, IResetSingletonHook
    {
        private readonly LinkedList<ServerLogEvent> _serverLogs = new();
        private readonly object _lock = new();

        /// <summary>
        /// Gets the expected logs.
        /// </summary>
        public LogFilter<ServerLogEvent> ExpectedLogs { get; }

        /// <summary>
        /// Check whether unexpected log event occured.
        /// </summary>
        public bool UnexpectedLogOccured { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public LogSnifferService(LTestConfiguration configuration)
        {
            ExpectedLogs = configuration.LogSniffer.DefaultExpectedEvents;
        }

        /// <summary>
        /// Saves log event into memory and checks whether it was unexpected.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        public bool CheckLogEvent(ServerLogEvent logEvent)
        {
            lock (_lock)
            {
                _serverLogs.AddLast(logEvent);

                if (!ExpectedLogs.Filters.Any(x => x.Action(logEvent)))
                {
                    UnexpectedLogOccured = true;
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Returns the actual snapshot of the events.
        /// </summary>
        public IReadOnlyCollection<ServerLogEvent> GetServerLogs()
        {
            lock (_lock)
            {
                return _serverLogs.ToList();
            }
        }

        /// <summary>
        /// Resets the service.
        /// </summary>
        public Task ResetAsync()
        {
            lock (_lock)
            {
                _serverLogs.Clear();
                ExpectedLogs.Reset();
                UnexpectedLogOccured = false;
            }

            return Task.CompletedTask;
        }
    }
}