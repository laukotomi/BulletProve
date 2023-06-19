using BulletProve.ServerLog;
using BulletProve.TestServer;
using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// Logger factory for LogSniffer.
    /// </summary>
    public sealed class TestLoggerFactory : ILoggerFactory
    {
        private readonly ServerConfigurator _configurator;
        private readonly ITestLogger _testLogger;
        private readonly IEnumerable<IServerLogInspector> _serverLogInspectors;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerFactory"/> class.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="testLogger">The test logger.</param>
        /// <param name="serverLogInspectors">The server log inspectors.</param>
        public TestLoggerFactory(
            ServerConfigurator configurator,
            ITestLogger testLogger,
            IEnumerable<IServerLogInspector> serverLogInspectors)
        {
            _configurator = configurator;
            _testLogger = testLogger;
            _serverLogInspectors = serverLogInspectors;
        }

        /// <inheritdoc />
        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerProvider(categoryName, _serverLogInspectors, _configurator, _testLogger);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}