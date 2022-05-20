using LTest.Logging;
using Microsoft.Extensions.Logging;

namespace LTest.Configuration
{
    /// <summary>
    /// The log sniffer configuration.
    /// </summary>
    public class LogSnifferConfiguration
    {
        /// <summary>
        /// Decide whether a server log event is expected or the test should fail. By default only log levels of information or smaller are allowed.
        /// </summary>
        public List<LogEventFilter<ServerLogEvent>> DefaultExpectedEvents { get; } = new List<LogEventFilter<ServerLogEvent>>
        {
            new LogEventFilter<ServerLogEvent>("LogLevelBelowWarning", (logEvent) => logEvent.Level < LogLevel.Warning)
        };
    }
}
