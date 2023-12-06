using BulletProve.ServerLog;
using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// The test logger provider.
    /// </summary>
    internal class LoggerProvider : ILogger
    {
        private readonly string _categoryName;
        private readonly ScopeProvider _scopeProvider;
        private readonly IEnumerable<IServerLogHandler> _serverLogHandlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerProvider"/> class.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <param name="scopeProvider">The scope provider.</param>
        /// <param name="serverLogHandlers">The server log handlers.</param>
        public LoggerProvider(string categoryName, ScopeProvider scopeProvider, IEnumerable<IServerLogHandler> serverLogHandlers)
        {
            _categoryName = categoryName;
            _scopeProvider = scopeProvider;
            _serverLogHandlers = serverLogHandlers;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (exception != null)
            {
                message += $"{Environment.NewLine}{exception}";
            }

            var serverLogEvent = new ServerLogEvent(_categoryName, logLevel, eventId, message, _scopeProvider.CurrentScope, exception);
            serverLogEvent.IsUnexpected = !_serverLogHandlers.Any(x => x.IsAllowed(serverLogEvent));

            foreach (var handler in _serverLogHandlers)
            {
                handler.HandleServerLog(serverLogEvent);
            }
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
#if NET7_0_OR_GREATER
            where TState : notnull
#endif
        {
            return _scopeProvider.CreateScope(_categoryName, state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel) => true;
    }
}