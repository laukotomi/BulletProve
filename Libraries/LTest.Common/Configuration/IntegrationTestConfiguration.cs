using LTest.Interfaces;
using LTest.LogSniffer;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LTest.Configuration
{
    /// <summary>
    /// Configuration for an integration test.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IntegrationTestConfiguration
    {
        /// <summary>
        /// Minimum log level to check for LogSniffer.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Turns off default LogSniffer registration and preserves the logger providers.
        /// </summary>
        public bool PreserveLoggerProviders { get; set; }

        /// <summary>
        /// Decide whether a log sniffer event is expected or the test should fail.
        /// </summary>
        public Func<LogSnifferEvent, bool> DefaultIsExpectedLogSnifferEventAction { get; set; } =
            (logEvent) => logEvent.Level <= LogLevel.Information;

        /// <summary>
        /// Configuration files to merge.
        /// </summary>
        public List<string> ConfigurationFiles { get; set; } = new();

        /// <summary>
        /// Whether to log logger category names at the end of the test. Useful for setting up the <see cref="ServerLogFilter"/>.
        /// </summary>
        public bool LogLoggerCategoryNames { get; set; }

        /// <summary>
        /// Logger filter.
        /// </summary>
        public IServerLogFilter ServerLogFilter { get; set; }
    }
}