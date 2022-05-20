using LTest.Configuration;
using LTest.Hooks;
using Microsoft.Extensions.Logging;

namespace LTest.Logging
{
    /// <summary>
    /// Test logger implementation.
    /// </summary>
    internal class TestLogger : ITestLogger, IResetSingletonHook
    {
        private readonly LinkedList<LTestLogEvent> _logs = new();
        private readonly object _lock = new();
        private readonly LTestConfiguration _configuration;
        private int _scopeLevel = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLogger"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public TestLogger(LTestConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        public void Log(LogLevel level, string message)
        {
            lock (_lock)
            {
                _logs.AddLast(new LTestLogEvent(level, _scopeLevel, message));
            }
        }

        /// <summary>
        /// Retrieves all log events.
        /// </summary>
        public IReadOnlyCollection<LTestLogEvent> GetSnapshot()
        {
            lock (_lock)
            {
                return _logs.ToList();
            }
        }

        /// <summary>
        /// Creates scope (indenting) in the logger.
        /// </summary>
        public TestLoggerScope Scope(Action<ITestLogger>? logAction = null)
        {
            logAction?.Invoke(this);

            if (!_configuration.DisableLogScoping)
            {
                _scopeLevel++;
            }

            return new TestLoggerScope(this, _scopeLevel, (scope) =>
            {
                if (!_configuration.DisableLogScoping)
                {
                    _scopeLevel--;

                    if (_scopeLevel != scope.Level - 1)
                    {
                        throw new InvalidOperationException($"Scope was created with level {scope.Level} but disposed to level {_scopeLevel}. Perhaps a previous scope was not properly disposed.");
                    }
                }
            });
        }

        /// <summary>
        /// Writes empty line.
        /// </summary>
        public void LogEmptyLine()
        {
            lock (_lock)
            {
                _logs.AddLast(new LTestLogEvent(LogLevel.None, _scopeLevel, string.Empty));
            }
        }

        /// <summary>
        /// Resets the service.
        /// </summary>
        /// <returns>A Task.</returns>
        public Task ResetAsync()
        {
            lock (_lock)
            {
                _logs.Clear();
                _scopeLevel = 0;
            }
            return Task.CompletedTask;
        }
    }
}