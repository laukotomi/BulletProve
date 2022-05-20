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
        /// <summary>
        /// The nr of events to log on error.
        /// </summary>
        private const int _nrOfEventsToLogOnError = 10;

        private readonly LinkedList<ServerLogEvent> _serverLogs;
        private readonly object _lock = new();
        private readonly ITestLogger _logger;
        private readonly LogFilter<ServerLogEvent> _expectedLogEventStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
        public LogSnifferService(ITestLogger logger, LTestConfiguration configuration)
        {
            _serverLogs = new LinkedList<ServerLogEvent>();
            _expectedLogEventStore = new LogFilter<ServerLogEvent>(configuration.LogSniffer.DefaultExpectedEvents);
            _logger = logger;
        }

        /// <summary>
        /// Check whether unexpected log event occured.
        /// </summary>
        public bool UnexpectedLogOccured { get; private set; }

        /// <summary>
        /// Overrides the is expected event action. It lasts only for the actual test.
        /// </summary>
        /// <param name="action">Action.</param>
        public ResetExpectedServerLogs ModifyExpectedLogEvents(Action<LogFilter<ServerLogEvent>> action)
        {
            lock (_lock)
            {
                action?.Invoke(_expectedLogEventStore);
            }

            return new ResetExpectedServerLogs(_expectedLogEventStore);
        }

        /// <summary>
        /// Saves log event into memory and checks whether it was unexpected.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        public void CheckLogEvent(ServerLogEvent logEvent)
        {
            lock (_lock)
            {
                _serverLogs.AddLast(logEvent);

                if (!_expectedLogEventStore.Filters.Any(x => x.Action(logEvent)))
                {
                    _logger.Log(logEvent.Level, logEvent.Message);

                    UnexpectedLogOccured = true;
                    LogLastElements();
                }
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
                _expectedLogEventStore.Reset();
                UnexpectedLogOccured = false;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs last server messages.
        /// </summary>
        private void LogLastElements()
        {
            var ev = _serverLogs.Last;
            for (var i = 0; i < _nrOfEventsToLogOnError && ev!.Previous != null; i++)
            {
                ev = ev.Previous;
            }

            while (ev != null)
            {
                var logEvent = ev.Value;
                _logger.Log(logEvent.Level, $"{logEvent.Message} [{logEvent.CategoryName}] [{logEvent.EventId}]");

                ev = ev.Next;
            }
        }
    }
}