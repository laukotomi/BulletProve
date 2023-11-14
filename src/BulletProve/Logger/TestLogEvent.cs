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
        public TestLogEvent(string prefix, LogLevel level, string message, Scope? scope = null)
        {
            Prefix = prefix;
            Level = level;
            Message = message;
            Scope = scope;
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Prefix to add.
        /// </summary>
        public string Prefix { get; }

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

        /// <summary>
        /// Gets the created at.
        /// </summary>
        public DateTime CreatedAt { get; }
    }
}