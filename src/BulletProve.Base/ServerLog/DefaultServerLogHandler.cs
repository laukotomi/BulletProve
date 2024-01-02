using BulletProve.Base.Configuration;
using BulletProve.Logging;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// The default server log handler.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DefaultServerLogHandler"/> class.
    /// </remarks>
    /// <param name="configurator">The configurator.</param>
    /// <param name="logger">The logger.</param>
    public class DefaultServerLogHandler(LoggerConfigurator configurator, ITestLogger logger, IServerLogCollector serverLogCollector) : IServerLogHandler
    {
        /// <inheritdoc/>
        public void HandleServerLog(ServerLogEvent serverLogEvent)
        {
            var logEvent = new TestLogEvent(serverLogEvent.CategoryName, serverLogEvent.Level, serverLogEvent.Message, !serverLogEvent.IsUnexpected, serverLogEvent.Scope);
            var logged = false;

            if (configurator.MinimumLogLevel <= serverLogEvent.Level && IsCategoryAllowed(serverLogEvent))
            {
                logger.Log(logEvent);
                logged = true;
            }

            if (serverLogEvent.IsUnexpected && !logged)
            {
                logger.Log(logEvent);
            }

            serverLogCollector.AddServerLog(serverLogEvent);
        }

        /// <inheritdoc/>
        public bool IsAllowed(ServerLogEvent item)
        {
            return configurator.ServerLogInspector.IsAllowed(item);
        }

        /// <summary>
        /// Is the category allowed.
        /// </summary>
        /// <param name="serverLogEvent">The server log event.</param>
        private bool IsCategoryAllowed(ServerLogEvent serverLogEvent)
        {
            return configurator.LoggerCategoryNameInspector.IsAllowed(serverLogEvent.CategoryName);
        }
    }
}
