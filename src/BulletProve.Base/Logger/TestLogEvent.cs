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
        /// <param name="category">The category.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="isExpected">If true, is expected.</param>
        /// <param name="scope">The scope.</param>
        public TestLogEvent(string category, LogLevel level, string message, bool isExpected, Scope? scope = null)
        {
            Category = category;
            Level = level;
            Message = message;
            IsExpected = isExpected;
            Scope = scope;
            CreatedAt = DateTime.Now;
        }

        /// <summary>
        /// Prefix to add.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the created at.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Gets a value indicating whether is expected.
        /// </summary>
        public bool IsExpected { get; }

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

        /// <inheritdoc />
        public override string ToString()
        {
            var unexpected = !IsExpected ? "U" : string.Empty;
            var level = Level.ToString()[0];

            return $"{level}{unexpected}: {Message} [{Category}]";
        }
    }
}