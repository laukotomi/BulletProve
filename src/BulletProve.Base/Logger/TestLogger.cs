using BulletProve.Hooks;
using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// Test logger implementation.
    /// </summary>
    internal class TestLogger : ITestLogger, ICleanUpHook
    {
        /// <summary>
        /// Prefix for test logs.
        /// </summary>
        public const string Category = nameof(TestLogger);

        private readonly LinkedList<TestLogEvent> _logs = new();
        private readonly object _lock = new();
        private readonly ScopeProvider _scopeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLogger"/> class.
        /// </summary>
        /// <param name="scopeProvider">The scope provider.</param>
        public TestLogger(ScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        /// <inheritdoc />
        public void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <inheritdoc />
        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <inheritdoc />
        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        /// <inheritdoc />
        public void Log(LogLevel level, string message)
        {
            Log(new TestLogEvent(Category, level, message, true, _scopeProvider.CurrentScope));
        }

        /// <inheritdoc />
        public void Log(TestLogEvent logEvent)
        {
            lock (_lock)
            {
                _logs.AddLast(logEvent);
            }
        }

        /// <summary>
        /// Retrieves all log events.
        /// </summary>
        public IReadOnlyList<TestLogEvent> GetSnapshot()
        {
            lock (_lock)
            {
                return _logs.ToList();
            }
        }

        /// <inheritdoc />
        public IDisposable Scope(object? state = null)
        {
            return _scopeProvider.CreateScope(Category, state);
        }

        /// <inheritdoc />
        public void LogEmptyLine()
        {
            Log(LogLevel.None, string.Empty);
        }

        /// <inheritdoc />
        public Task CleanUpAsync()
        {
            lock (_lock)
            {
                _logs.Clear();
            }

            return Task.CompletedTask;
        }
    }
}