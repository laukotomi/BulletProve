using BulletProve.Logging;
using Microsoft.Extensions.Logging;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// Class for a log event.
    /// </summary>
    public class ServerLogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLogEvent"/> class.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <param name="level">Log level.</param>
        /// <param name="eventId">Event id.</param>
        /// <param name="message">Log message.</param>
        /// <param name="exception">Exception attached to the log.</param>
        public ServerLogEvent(string categoryName, LogLevel level, EventId eventId, string message, Scope? scope, Exception? exception)
        {
            CategoryName = categoryName;
            EventId = eventId;
            Level = level;
            Message = message;
            Scope = scope;
            Exception = exception;
        }

        /// <summary>
        /// Category name.
        /// </summary>
        public string CategoryName { get; }

        /// <summary>
        /// Event Id.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        /// Exception attached to the log event.
        /// </summary>
        public Exception? Exception { get; }

        /// <summary>
        /// Gets or sets a value indicating whether is unexpected.
        /// </summary>
        public bool IsUnexpected { get; set; }

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
            var unexpected = IsUnexpected ? "U" : string.Empty;
            var level = Level.ToString()[0];

            return $"{level}{unexpected}: {Message} [{CategoryName}] [{EventId}]";
        }
    }
}