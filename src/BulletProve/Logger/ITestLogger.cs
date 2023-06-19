using Microsoft.Extensions.Logging;

namespace BulletProve.Logging
{
    /// <summary>
    /// Test logger interface.
    /// </summary>
    public interface ITestLogger
    {
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void LogError(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void LogWarning(string message);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void LogInformation(string message);

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Logs a <see cref="TestLogEvent"/>.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        void Log(TestLogEvent logEvent);

        /// <summary>
        /// Creates scope (indenting) in the logger.
        /// </summary>
        IDisposable Scope(object? state = null);

        /// <summary>
        /// Writes empty line.
        /// </summary>
        void LogEmptyLine();

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        Scope? GetCurrentScope();
    }
}