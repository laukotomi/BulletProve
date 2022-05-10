using System;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Helper class to reset LogSniffer expected log event action.
    /// </summary>
    public sealed class ResetExpectedLogEventAction : IDisposable
    {
        private readonly LogSnifferService _logSnifferService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetExpectedLogEventAction"/> class.
        /// </summary>
        /// <param name="logSnifferService">LogSniffer service</param>
        public ResetExpectedLogEventAction(LogSnifferService logSnifferService)
        {
            _logSnifferService = logSnifferService;
        }

        /// <summary>
        /// Resets LogSniffer.
        /// </summary>
        public void Dispose()
        {
            _logSnifferService.OverrideIsExpectedLogEventAction(null);
        }
    }
}