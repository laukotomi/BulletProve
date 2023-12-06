using BulletProve.ServerLog;
using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// Logger factory for LogSniffer.
    /// </summary>
    public sealed class TestLoggerFactory : ILoggerFactory
    {
        private readonly IEnumerable<IServerLogHandler> _serverLogHandlers;
        private readonly ScopeProvider _scopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerFactory"/> class.
        /// </summary>
        /// <param name="serverLogHandlers">The server log handlers.</param>
        /// <param name="scopeProvider">The scope provider.</param>
        public TestLoggerFactory(
            IEnumerable<IServerLogHandler> serverLogHandlers,
            ScopeProvider scopeProvider)
        {
            _serverLogHandlers = serverLogHandlers;
            _scopeProvider = scopeProvider;
        }

        /// <inheritdoc />
        public void AddProvider(ILoggerProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new LoggerProvider(categoryName, _scopeProvider, _serverLogHandlers);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}