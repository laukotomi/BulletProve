using LTest.Logger;
using Microsoft.Extensions.Logging;
using System;

namespace LTest.Interfaces
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
        void Error(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Warning(string message);

        /// <summary>
        /// Logs an information message.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Info(string message);

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="message">Log message.</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Resets the service.
        /// </summary>
        void Reset();

        /// <summary>
        /// Creates scope (indenting) in the logger.
        /// </summary>
        TestLoggerScope Scope(Action<ITestLogger> logAction);

        /// <summary>
        /// Writes empty line.
        /// </summary>
        void EmptyLine();
    }
}