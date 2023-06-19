using BulletProve.ServerLog;
using BulletProve.TestServer;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;

namespace BulletProve.Logging
{
    /// <summary>
    /// The test logger provider.
    /// </summary>
    internal class LoggerProvider : ILogger
    {
        private readonly string _categoryName;
        private readonly IEnumerable<IServerLogInspector> _serverLogInspectors;
        private readonly ServerConfigurator _configurator;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <param name="logSniffer">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        public LoggerProvider(string categoryName, IEnumerable<IServerLogInspector> serverLogInspectors, ServerConfigurator configurator, ITestLogger logger)
        {
            _categoryName = categoryName;
            _serverLogInspectors = serverLogInspectors;
            _configurator = configurator;
            _logger = logger;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (exception != null)
            {
                message += $"{Environment.NewLine}{exception}";
            }

            var serverLogEvent = new ServerLogEvent(_categoryName, logLevel, eventId, message, _logger.GetCurrentScope(), exception);
            CheckExpected(serverLogEvent);

            var logEvent = new TestLogEvent(serverLogEvent.Level, serverLogEvent.Message, serverLogEvent.IsUnexpected, serverLogEvent.Scope);
            var logged = false;

            if (_configurator.MinimumLogLevel <= logLevel)
            {
                if (_configurator.LoggerCategoryNameInspector.IsAllowed(_categoryName))
                {
                    _logger.Log(logEvent);
                    logged = true;
                }

                Debug.WriteLine(serverLogEvent.ToString());
            }

            if (logEvent.IsUnexpected && !logged)
            {
                _logger.Log(logEvent);
            }
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
#if NET7_0
            where TState : notnull
#endif
        {
            if (_configurator.LoggerCategoryNameInspector.IsAllowed(_categoryName))
            {
                _logger.LogInformation($"Scope: {JsonSerializer.Serialize(state)}");
            }

            return _logger.Scope(state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Checks if the log event is expected.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        private void CheckExpected(ServerLogEvent logEvent)
        {
            var expected = _serverLogInspectors.Any(x => x.IsServerLogEventAllowed(logEvent));
            if (!expected)
            {
                logEvent.IsUnexpected = true;
            }
        }
    }
}