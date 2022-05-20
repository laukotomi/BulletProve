using LTest.Configuration;
using LTest.LogSniffer;
using Microsoft.Extensions.Logging;

namespace LTest.Logging
{
    /// <summary>
    /// Logger factory for LogSniffer.
    /// </summary>
    public sealed class LTestLoggerFactory : ILoggerFactory
    {
        private readonly ILogSnifferService _logSnifferService;
        private readonly LTestConfiguration _configuration;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestLoggerFactory"/> class.
        /// </summary>
        /// <param name="logSnifferService">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="testLogger">Logger.</param>
        public LTestLoggerFactory(
            ILogSnifferService logSnifferService,
            LTestConfiguration configuration,
            ITestLogger testLogger)
        {
            _logSnifferService = logSnifferService;
            _configuration = configuration;
            _testLogger = testLogger;
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
            return new LTestLogger(categoryName, _logSnifferService, _configuration, _testLogger);
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