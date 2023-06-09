using LTest.LogSniffer;
using LTest.TestServer;
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
        private readonly IEnumerable<IServerLogInspector> _serverLogInspectors;
        private readonly ServerConfigurator _configurator;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestLogger"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="logSniffer">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        public LTestLogger(string categoryName, IEnumerable<IServerLogInspector> serverLogInspectors, ServerConfigurator configurator, ITestLogger logger)
        {
            _categoryName = categoryName;
            _serverLogInspectors = serverLogInspectors;
            _configurator = configurator;
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

            var logEvent = new ServerLogEvent(_categoryName, logLevel, eventId, message, _logger.GetCurrentScope(), exception);

            var expected = _serverLogInspectors.Any(x => x.IsServerLogEventAllowed(logEvent));
            if (!expected)
            {
                logEvent.IsUnexpected = true;
                message = "UNEXPECTED: " + message;
            }

            bool logged = false;
            if (_configurator.MinimumLogLevel <= logLevel)
            {
                if (_configurator.LoggerCategoryNameInspector.IsAllowed(_categoryName))
                {
                    _logger.Log(logLevel, message);
                    logged = true;
                }

                Debug.WriteLine(logEvent.ToString());
            }

            if (!expected && !logged)
            {
                _logger.Log(logLevel, message);
            }
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An System.IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            if (_configurator.LoggerCategoryNameInspector.IsAllowed(_categoryName))
            {
                _logger.LogInformation($"Scope: {JsonSerializer.Serialize(state)}");
            }

            return _logger.Scope(state);
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns>true if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel) => true;
    }
}