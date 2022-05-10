using LTest.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTest.Logger
{
    /// <summary>
    /// Test logger implementation.
    /// </summary>
    internal class TestLogger : ITestLogger
    {
        private readonly LinkedList<LogEvent> _logs = new LinkedList<LogEvent>();
        private readonly object _lock = new object();
        private int _scopeLevel = 0;

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">Log message.</param>
        public void Info(string message)
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
                _logs.AddLast(new LogEvent(level, _scopeLevel, message));
            }
        }

        /// <summary>
        /// Retrieves all log events.
        /// </summary>
        public IEnumerable<LogEvent> GetSnapshot()
        {
            lock (_lock)
            {
                return _logs.ToList();
            }
        }

        /// <summary>
        /// Resets the service.
        /// </summary>
        public void Reset()
        {
            lock (_lock)
            {
                _logs.Clear();
            }
        }

        /// <summary>
        /// Creates scope (indenting) in the logger.
        /// </summary>
        public TestLoggerScope Scope(Action<ITestLogger> logAction)
        {
            logAction(this);
            _scopeLevel++;

            return new TestLoggerScope(this, _scopeLevel, (scope) =>
            {
                _scopeLevel--;

                if (_scopeLevel != scope.Level - 1)
                {
                    throw new InvalidOperationException($"Scope was created with level {scope.Level} but disposed to level {_scopeLevel}. Perhabs a previous scope was not properly disposed.");
                }
            });
        }

        /// <summary>
        /// Writes empty line.
        /// </summary>
        public void EmptyLine()
        {
            lock (_lock)
            {
                _logs.AddLast(new LogEvent(LogLevel.None, _scopeLevel, string.Empty));
            }
        }
    }

    /// <summary>
    /// Test logger scope.
    /// </summary>
    public sealed class TestLoggerScope : IDisposable
    {
        private readonly ITestLogger _logger;
        private readonly Action<TestLoggerScope> _finishAction;
        private bool _finalized;

        /// <summary>
        /// Scope level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerScope"/> class.
        /// </summary>
        /// <param name="logger">Test logger.</param>
        /// <param name="level">Scope level</param>
        /// <param name="finishAction">Finish action.</param>
        public TestLoggerScope(ITestLogger logger, int level, Action<TestLoggerScope> finishAction)
        {
            _logger = logger;
            Level = level;
            _finishAction = finishAction;
        }

        /// <summary>
        /// Finish action (same as Dispose, but enables logging).
        /// </summary>
        public void Finish(Action<ITestLogger> logAction = null)
        {
            Dispose();
            logAction?.Invoke(_logger);

            if (Level <= 2)
            {
                _logger.EmptyLine();
            }
        }

        /// <summary>
        /// Dispose action.
        /// </summary>
        public void Dispose()
        {
            if (!_finalized)
            {
                _finishAction(this);
                _finalized = true;
            }
        }
    }
}