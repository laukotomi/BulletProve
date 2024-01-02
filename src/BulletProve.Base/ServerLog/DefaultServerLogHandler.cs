using BulletProve.Base.Configuration;
using BulletProve.Logging;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// The default server log handler.
    /// </summary>
    public class DefaultServerLogHandler : IServerLogHandler
    {
        private readonly LoggerConfigurator _configurator;
        private readonly ITestLogger _logger;
        private readonly IServerLogCollector _serverLogCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerLogHandler"/> class.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="logger">The logger.</param>
        public DefaultServerLogHandler(LoggerConfigurator configurator, ITestLogger logger, IServerLogCollector serverLogCollector)
        {
            _configurator = configurator;
            _logger = logger;
            _serverLogCollector = serverLogCollector;
        }

        /// <inheritdoc/>
        public void HandleServerLog(ServerLogEvent serverLogEvent)
        {
            var logEvent = new TestLogEvent(serverLogEvent.CategoryName, serverLogEvent.Level, serverLogEvent.Message, !serverLogEvent.IsUnexpected, serverLogEvent.Scope);
            var logged = false;

            if (_configurator.MinimumLogLevel <= serverLogEvent.Level && IsCategoryAllowed(serverLogEvent))
            {
                _logger.Log(logEvent);
                logged = true;
            }

            if (serverLogEvent.IsUnexpected && !logged)
            {
                _logger.Log(logEvent);
            }

            _serverLogCollector.AddServerLog(serverLogEvent);
        }

        /// <inheritdoc/>
        public bool IsAllowed(ServerLogEvent item)
        {
            return _configurator.ServerLogInspector.IsAllowed(item);
        }

        /// <summary>
        /// Is the category allowed.
        /// </summary>
        /// <param name="serverLogEvent">The server log event.</param>
        private bool IsCategoryAllowed(ServerLogEvent serverLogEvent)
        {
            return _configurator.LoggerCategoryNameInspector.IsAllowed(serverLogEvent.CategoryName);
        }
    }
}
