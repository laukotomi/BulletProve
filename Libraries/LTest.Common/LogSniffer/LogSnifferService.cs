using LTest.Configuration;
using LTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Log sniffer service implementation.
    /// </summary>
    public class LogSnifferService : ILogSnifferService
    {
        private const int NrOfEventsToLogOnError = 10;

        private readonly LinkedList<LogSnifferEvent> _logEvents;
        private readonly object _lock = new object();
        private readonly ITestLogger _logger;
        private readonly Func<LogSnifferEvent, bool> _isExpectedLogEventAction;
        private Func<LogSnifferEvent, bool> _overriddenIsExpectedLogEventAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferService"/> class.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
        public LogSnifferService(ITestLogger logger, IntegrationTestConfiguration configuration)
        {
            _logEvents = new LinkedList<LogSnifferEvent>();
            _logger = logger;
            _isExpectedLogEventAction = configuration.DefaultIsExpectedLogSnifferEventAction;
        }

        /// <summary>
        /// Check whether unexpected log event occured.
        /// </summary>
        public bool UnexpectedLogOccured { get; private set; }

        /// <summary>
        /// Overrides the is expected event action. It lasts only for the actual test.
        /// </summary>
        /// <param name="action">Action.</param>
        public ResetExpectedLogEventAction OverrideIsExpectedLogEventAction(Func<LogSnifferEvent, bool> action)
        {
            lock (_lock)
            {
                if (_overriddenIsExpectedLogEventAction == null || action == null)
                    _overriddenIsExpectedLogEventAction = action;
            }

            return new ResetExpectedLogEventAction(this);
        }

        /// <summary>
        /// Saves log event into memory and checks whether it was unexpected.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        public void AddLogEvent(LogSnifferEvent logEvent)
        {
            lock (_lock)
            {
                _logEvents.AddLast(logEvent);

                var isExpectedLogAction = _overriddenIsExpectedLogEventAction ?? _isExpectedLogEventAction;
                if (isExpectedLogAction != null && !isExpectedLogAction.Invoke(logEvent))
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
        public List<LogSnifferEvent> GetSnapshot()
        {
            lock (_lock)
            {
                return _logEvents.ToList();
            }
        }

        /// <summary>
        /// Resets the service.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _logEvents.Clear();
                _overriddenIsExpectedLogEventAction = null;
                UnexpectedLogOccured = false;
            }
        }

        private void LogLastElements()
        {
            var ev = _logEvents.Last;
            for (var i = 0; i < NrOfEventsToLogOnError && ev.Previous != null; i++)
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