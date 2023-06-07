using Microsoft.Extensions.Logging;

namespace LTest.Logging
{
    /// <summary>
    /// Log event.
    /// </summary>
    public class LTestLogEvent
    {
        public LTestLogEvent(LogLevel level, string message, Scope? scope)
        {
            Level = level;
            Message = message;
            Scope = scope;
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Log level.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public Scope? Scope { get; }

        public DateTime CreatedAt { get; }
    }
}