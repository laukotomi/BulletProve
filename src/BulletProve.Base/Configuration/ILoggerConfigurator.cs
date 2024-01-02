using BulletProve.ServerLog;
using BulletProve.Services;
using Microsoft.Extensions.Logging;

namespace BulletProve.Base.Configuration
{
    /// <summary>
    /// The logger configurator.
    /// </summary>
    public interface ILoggerConfigurator
    {
        /// <summary>
        /// Configures the allowed logger category names.
        /// </summary>
        /// <param name="action">The action.</param>
        ILoggerConfigurator ConfigureAllowedLoggerCategoryNames(Action<Inspector<string>> action);

        /// <summary>
        /// Configures the server log inspector.
        /// </summary>
        /// <param name="action">The action.</param>
        ILoggerConfigurator ConfigureServerLogInspector(Action<Inspector<ServerLogEvent>> action);

        /// <summary>
        /// Sets whether to log the category names.
        /// </summary>
        /// <param name="logCategoryNames">If true, logs category names.</param>
        ILoggerConfigurator SetLogCategoryNames(bool logCategoryNames);

        /// <summary>
        /// Sets the minimum log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        ILoggerConfigurator SetMinimumLogLevel(LogLevel logLevel);
    }
}
