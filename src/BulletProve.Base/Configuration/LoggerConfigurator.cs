using BulletProve.Base.Hooks;
using BulletProve.ServerLog;
using BulletProve.Services;
using Microsoft.Extensions.Logging;

namespace BulletProve.Base.Configuration
{
    /// <summary>
    /// The logger configurator.
    /// </summary>
    public class LoggerConfigurator : ILoggerConfigurator, ICleanUpHook
    {
        /// <summary>
        /// Gets a value indicating whether log category names.
        /// </summary>
        public bool LogCategoryNames { get; private set; } = false;

        /// <summary>
        /// Gets the logger category name inspector.
        /// </summary>
        public Inspector<string> LoggerCategoryNameInspector { get; } = new();

        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        public LogLevel MinimumLogLevel { get; private set; } = LogLevel.Information;

        /// <summary>
        /// Gets the server log inspector.
        /// </summary>
        public Inspector<ServerLogEvent> ServerLogInspector { get; } = new();

        /// <inheritdoc/>
        public ILoggerConfigurator ConfigureAllowedLoggerCategoryNames(Action<Inspector<string>> action)
        {
            action(LoggerCategoryNameInspector);
            return this;
        }

        /// <inheritdoc/>
        public ILoggerConfigurator ConfigureServerLogInspector(Action<Inspector<ServerLogEvent>> action)
        {
            action(ServerLogInspector);
            return this;
        }

        /// <inheritdoc/>
        public ILoggerConfigurator SetLogCategoryNames(bool logCategoryNames)
        {
            LogCategoryNames = logCategoryNames;
            return this;
        }

        /// <inheritdoc/>
        public ILoggerConfigurator SetMinimumLogLevel(LogLevel logLevel)
        {
            MinimumLogLevel = logLevel;
            return this;
        }

        /// <inheritdoc/>
        public async Task CleanUpAsync()
        {
            await LoggerCategoryNameInspector.CleanUpAsync();
            await ServerLogInspector.CleanUpAsync();
        }
    }
}
