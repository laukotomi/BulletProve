using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// Log event.
    /// </summary>
    public class TestLogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestLogEvent"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="scope">The scope.</param>
        public TestLogEvent(LogLevel level, string message, bool isUnexpected, Scope? scope = null)
        {
            Level = level;
            Message = message;
            IsUnexpected = isUnexpected;
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
        /// Gets a value indicating whether is unexpected event.
        /// </summary>
        public bool IsUnexpected { get; }

        /// <summary>
        /// Gets the scope.
        /// </summary>
        public Scope? Scope { get; }

        /// <summary>
        /// Gets the created at.
        /// </summary>
        public DateTime CreatedAt { get; }
    }
}