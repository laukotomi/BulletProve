using LTest.LogSniffer;
using LTest.TestServer;
using Microsoft.Extensions.Logging;

namespace LTest.Logging
{
    /// <summary>
    /// Logger factory for LogSniffer.
    /// </summary>
    public sealed class LTestLoggerFactory : ILoggerFactory
    {
        private readonly ServerConfigurator _configurator;
        private readonly ITestLogger _testLogger;
        private readonly IEnumerable<IServerLogInspector> _serverLogInspectors;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestLoggerFactory"/> class.
        /// </summary>
        /// <param name="logSnifferService">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="testLogger">Logger.</param>
        public LTestLoggerFactory(
            ServerConfigurator configurator,
            ITestLogger testLogger,
            IEnumerable<IServerLogInspector> serverLogInspectors)
        {
            _configurator = configurator;
            _testLogger = testLogger;
            _serverLogInspectors = serverLogInspectors;
        }

        /// <summary>
        /// Not in use.
        /// </summary>
        /// <param name="provider">Logger provider.</param>
        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates logger for category.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        public ILogger CreateLogger(string categoryName)
        {
            return new LTestLogger(categoryName, _serverLogInspectors, _configurator, _testLogger);
        }

        /// <summary>
        /// IDisposable implementation.
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}