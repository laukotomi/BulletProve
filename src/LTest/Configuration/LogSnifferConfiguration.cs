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
        /// Initializes a new instance of the <see cref="LogSnifferConfiguration"/> class.
        /// </summary>
        public LogSnifferConfiguration()
        {
            DefaultExpectedEvents = new LogFilter<ServerLogEvent>(new List<LogEventFilter<ServerLogEvent>>
            {
                new LogEventFilter<ServerLogEvent>("LogLevelBelowWarning", (logEvent) => logEvent.Level < LogLevel.Warning)
            });
        }

        /// <summary>
        /// Decide whether a server log event is expected or the test should fail. By default only log levels of information or smaller are allowed.
        /// </summary>
        public LogFilter<ServerLogEvent> DefaultExpectedEvents { get; }
    }
}
