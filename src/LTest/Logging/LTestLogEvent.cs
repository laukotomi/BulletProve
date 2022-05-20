using Microsoft.Extensions.Logging;

namespace LTest.Logging
{
    /// <summary>
    /// Log event.
    /// </summary>
    internal class LTestLogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LTestLogEvent"/> class.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="scopeLevel">Scope level</param>
        /// <param name="message">Log message.</param>
        public LTestLogEvent(LogLevel level, int scopeLevel, string message)
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