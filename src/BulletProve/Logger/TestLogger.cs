using BulletProve.Hooks;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

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
        private const string Prefix = "T ";

        private readonly LinkedList<TestLogEvent> _logs = new();
        private readonly object _lock = new();

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        public AsyncLocal<Scope?> CurrentScope { get; private set; } = new();

        /// <summary>
        /// Gets the scopes that were created during the test.
        /// </summary>
        public ConcurrentBag<Scope> Scopes { get; } = new();

        /// <inheritdoc />
        public Scope? GetCurrentScope() => CurrentScope.Value;

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
            Log(new TestLogEvent(Prefix, level, message, CurrentScope.Value));
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
        public IReadOnlyCollection<TestLogEvent> GetSnapshot()
        {
            lock (_lock)
            {
                return _logs.ToList();
            }
        }

        /// <inheritdoc />
        public IDisposable Scope(object? state = null)
        {
            Scope? parent = CurrentScope.Value;
            var newScope = new Scope(this, state, parent);
            Scopes.Add(newScope);
            CurrentScope.Value = newScope;

            return newScope;
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
                CurrentScope = new();
                Scopes.Clear();
            }

            return Task.CompletedTask;
        }
    }
}