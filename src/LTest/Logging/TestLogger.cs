using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LTest.Logging
{
    /// <summary>
    /// Test logger implementation.
    /// </summary>
    internal class TestLogger : ITestLogger
    {
        private readonly LinkedList<LTestLogEvent> _logs = new();
        private readonly object _lock = new();

        public AsyncLocal<Scope?> CurrentScope { get; private set; } = new();

        public ConcurrentBag<Scope> Scopes { get; } = new();

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
                _logs.AddLast(new LTestLogEvent(level, message, CurrentScope.Value));
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
        public IDisposable Scope(object? state = null)
        {
            Scope? parent = CurrentScope.Value;
            var newScope = new Scope(this, state, parent);
            Scopes.Add(newScope);
            CurrentScope.Value = newScope;

            return newScope;
        }

        /// <summary>
        /// Writes empty line.
        /// </summary>
        public void LogEmptyLine()
        {
            Log(LogLevel.None, string.Empty);
        }

        public Scope? GetCurrentScope() => CurrentScope.Value;

        /// <summary>
        /// Resets the service.
        /// </summary>
        /// <returns>A Task.</returns>
        public void Clear()
        {
            lock (_lock)
            {
                _logs.Clear();
                CurrentScope = new();
                Scopes.Clear();
            }
        }
    }
}