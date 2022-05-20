using LTest.Configuration;
using LTest.LogSniffer;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace LTest.Logging
{
    /// <summary>
    /// The LTest logger.
    /// </summary>
    internal class LTestLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ILogSnifferService _logSniffer;
        private readonly LTestConfiguration _configuration;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="logSniffer">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        public LTestLogger(string categoryName, ILogSnifferService logSniffer, LTestConfiguration configuration, ITestLogger logger)
        {
            _categoryName = categoryName;
            _logSniffer = logSniffer;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a System.String message of the state and exception.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (exception != null)
            {
                message += $"{Environment.NewLine}{exception}";
            }

            var logEvent = new ServerLogEvent(_categoryName, logLevel, eventId, message, exception);

            _logSniffer.CheckLogEvent(logEvent);

            if (_configuration.MinimumLogLevel <= logLevel)
            {
                if (_configuration.ServerLogFilter.Filters.Any(x => x.Action(_categoryName)))
                {
                    _logger.Log(logLevel, message);
                }

                Debug.WriteLine(logEvent.ToString());
            }
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An System.IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            if (_configuration.ServerLogFilter.Filters.Any(x => x.Action(_categoryName)))
            {
                var loggerScope = _logger.Scope(logger => logger.LogInformation($"Scope: {JsonSerializer.Serialize(state)}"));
                return new LoggerScope(loggerScope);
            }

            return new LoggerScope();
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>true if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Helper class for BeginScope method.
        /// </summary>
        private sealed class LoggerScope : IDisposable
        {
            private readonly TestLoggerScope? _scope;

            /// <summary>
            /// Initializes a new instance of the <see cref="LoggerScope"/> class.
            /// </summary>
            /// <param name="scope">The scope.</param>
            public LoggerScope(TestLoggerScope? scope = null)
            {
                _scope = scope;
            }

            /// <summary>
            /// IDisposable implementation.
            /// </summary>
            public void Dispose()
            {
                _scope?.Dispose();
            }
        }
    }
}