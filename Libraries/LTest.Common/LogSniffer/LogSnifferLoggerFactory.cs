using LTest.Configuration;
using LTest.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Logger factory for LogSniffer.
    /// </summary>
    public sealed class LogSnifferLoggerFactory : ILoggerFactory
    {
        private readonly ILogSnifferService _logSnifferService;
        private readonly IntegrationTestConfiguration _configuration;
        private readonly ITestLogger _logger;
        private readonly CategoryNameCollector _categoryNameCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferLoggerFactory"/> class.
        /// </summary>
        /// <param name="logSnifferService">LogSniffer service.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="categoryNameCollector">Category name collector.</param>
        public LogSnifferLoggerFactory(
            ILogSnifferService logSnifferService,
            IntegrationTestConfiguration configuration,
            ITestLogger logger,
            CategoryNameCollector categoryNameCollector)
        {
            _logSnifferService = logSnifferService;
            _configuration = configuration;
            _logger = logger;
            _categoryNameCollector = categoryNameCollector;
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
            _categoryNameCollector.AddCategoryName(categoryName);
            return new LogSnifferLogger(categoryName, _logSnifferService, _configuration, _logger);
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