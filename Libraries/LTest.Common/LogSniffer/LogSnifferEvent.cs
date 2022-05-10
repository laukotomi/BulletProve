using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Class for a log event.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class LogSnifferEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferEvent"/> class.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        /// <param name="level">Log level.</param>
        /// <param name="eventId">Event id.</param>
        /// <param name="message">Log message.</param>
        public LogSnifferEvent(string categoryName, LogLevel level, EventId eventId, string message)
        {
            CategoryName = categoryName;
            EventId = eventId;
            Level = level;
            Message = message;
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
        /// Log level.
        /// </summary>
        public LogLevel Level { get; }

        /// <summary>
        /// Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// ToString() override.
        /// </summary>
        public override string ToString()
        {
            return $"{Level.ToString()[0]}: {Message} [{CategoryName}] [{EventId}]";
        }
    }
}