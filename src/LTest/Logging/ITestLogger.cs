using Microsoft.Extensions.Logging;

namespace LTest.Logging
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
        /// Creates scope (indenting) in the logger.
        /// </summary>
        TestLoggerScope Scope(Action<ITestLogger> logAction);

        /// <summary>
        /// Writes empty line.
        /// </summary>
        void LogEmptyLine();
    }
}