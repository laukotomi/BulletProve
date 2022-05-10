using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace LTest.Logger
{
    /// <summary>
    /// Log event.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class LogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="scopeLevel">Scope level</param>
        /// <param name="message">Log message.</param>
        public LogEvent(LogLevel level, int scopeLevel, string message)
        {
            Level = level;
            ScopeLevel = scopeLevel;
            Message = message;
        }

        /// <summary>
        /// Log level.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Scope level.
        /// </summary>
        public int ScopeLevel { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// ToString() override.
        /// </summary>
        public override string ToString()
        {
            if (Level == LogLevel.None)
                return string.Empty;

            return $"{Level.ToString()[0]}: {new string(' ', ScopeLevel * 2)}{Message}";
        }
    }
}