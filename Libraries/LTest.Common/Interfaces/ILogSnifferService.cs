using LTest.LogSniffer;
using System;
using System.Collections.Generic;

namespace LTest.Interfaces
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
        List<LogSnifferEvent> GetSnapshot();

        /// <summary>
        /// Saves log event into memory and checks wheter it was unexpected.
        /// </summary>
        /// <param name="logEvent">Log event.</param>
        void AddLogEvent(LogSnifferEvent logEvent);

        /// <summary>
        /// Overrides the is expected event action.
        /// </summary>
        /// <param name="action">Action.</param>
        ResetExpectedLogEventAction OverrideIsExpectedLogEventAction(Func<LogSnifferEvent, bool> action);

        /// <summary>
        /// Cleans the service.
        /// </summary>
        void Reset();
    }
}