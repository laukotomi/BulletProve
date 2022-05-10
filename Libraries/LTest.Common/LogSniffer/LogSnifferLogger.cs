using LTest.Configuration;
using LTest.Interfaces;
using LTest.Logger;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text.Json;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Logger for log sniffer.
    /// </summary>
    internal class LogSnifferLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ILogSnifferService _logSnifferService;
        private readonly IntegrationTestConfiguration _configuration;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="logSnifferService">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        public LogSnifferLogger(string categoryName, ILogSnifferService logSnifferService, IntegrationTestConfiguration configuration, ITestLogger logger)
        {
            _categoryName = categoryName;
            _logSnifferService = logSnifferService;
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
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_configuration.MinimumLogLevel > logLevel)
            {
                return;
            }

            var message = formatter(state, exception);
            if (exception != null)
            {
                message += $"{Environment.NewLine}{exception}";
            }

            var logSnifferEvent = new LogSnifferEvent(_categoryName, logLevel, eventId, message);

            _logSnifferService.AddLogEvent(logSnifferEvent);

            if (_configuration.ServerLogFilter?.Filter(_categoryName) ?? false)
            {
                _logger.Log(logLevel, message);
            }

            Debug.WriteLine(logSnifferEvent.ToString());
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An System.IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            if (_configuration.ServerLogFilter?.Filter(_categoryName) ?? false)
            {
                var loggerScope = _logger.Scope(logger => logger.Info($"Scope: {JsonSerializer.Serialize(state)}"));
                return new LoggerScope(loggerScope);
            }

            return new LoggerScope();
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>true if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel)
            => true;

        /// <summary>
        /// Helper class for BeginScope method.
        /// </summary>
        private sealed class LoggerScope : IDisposable
        {
            private readonly TestLoggerScope _scope;

            public LoggerScope(TestLoggerScope scope = null)
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