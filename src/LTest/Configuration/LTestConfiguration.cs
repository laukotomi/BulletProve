using LTest.Logging;
using Microsoft.Extensions.Logging;

namespace LTest.Configuration
{
    /// <summary>
    /// The LTest configuration.
    /// </summary>
    public class LTestConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LTestConfiguration"/> class.
        /// </summary>
        /// <param name="defaultLogFilter">The default log filter.</param>
        public LTestConfiguration(LogFilter<string> defaultLogFilter)
        {
            ServerLogFilter = defaultLogFilter;
        }

        /// <summary>
        /// Turns off LTestLogger registration to preserve the default logger. Also disables those features.
        /// </summary>
        public bool PreserveLoggerProviders { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether disable log scoping.
        /// </summary>
        public bool DisableLogScoping { get; set; }

        /// <summary>
        /// Minimum log level to log on debug window.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Configuration files to merge.
        /// </summary>
        public List<string> ConfigurationFiles { get; set; } = new();

        /// <summary>
        /// Gets the log sniffer configuration.
        /// </summary>
        public LogSnifferConfiguration LogSniffer { get; } = new();

        /// <summary>
        /// Filters the logs that will be written into the output window. The input is the logger category name.
        /// </summary>
        public LogFilter<string> ServerLogFilter { get; }
    }
}