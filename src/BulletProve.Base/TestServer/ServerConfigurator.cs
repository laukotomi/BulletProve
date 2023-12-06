using BulletProve.ServerLog;
using BulletProve.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BulletProve.TestServer
{
    /// <summary>
    /// The server configurator.
    /// </summary>
    public class ServerConfigurator
    {
        /// <summary>
        /// The localhost.
        /// </summary>
        private const string Localhost = "https://localhost/";

        /// <summary>
        /// Gets the app settings.
        /// </summary>
        public Dictionary<string, string> AppSettings { get; } = new();

        /// <summary>
        /// Gets the http client options.
        /// </summary>
        public WebApplicationFactoryClientOptions HttpClientOptions { get; } = new()
        {
            BaseAddress = new Uri(Localhost)
        };

        /// <summary>
        /// Gets the json configuration files.
        /// </summary>
        public List<string> JsonConfigurationFiles { get; } = new();

        /// <summary>
        /// Gets the logger category name inspector.
        /// </summary>
        public Inspector<string> LoggerCategoryNameInspector { get; } = new();

        /// <summary>
        /// Gets or sets the minimum log level.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Gets the server log inspector.
        /// </summary>
        public Inspector<ServerLogEvent> ServerLogInspector { get; } = new();

        /// <summary>
        /// Gets the service configurators.
        /// </summary>
        public List<Action<IServiceCollection>> ServiceConfigurators { get; } = new();

        /// <summary>
        /// Adds the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public ServerConfigurator AddAppSetting(string key, string value)
        {
            AppSettings.Add(key, value);
            return this;
        }

        /// <summary>
        /// Adds the json configuration file.
        /// </summary>
        /// <param name="file">The file.</param>
        public ServerConfigurator AddJsonConfigurationFile(string file)
        {
            JsonConfigurationFiles.Add(file);
            return this;
        }

        /// <summary>
        /// Configures the http client.
        /// </summary>
        /// <param name="action">The action.</param>
        public ServerConfigurator ConfigureHttpClient(Action<WebApplicationFactoryClientOptions> action)
        {
            action(HttpClientOptions);
            return this;
        }

        /// <summary>
        /// Configures the logger category name inspector.
        /// </summary>
        /// <param name="action">The action.</param>
        public ServerConfigurator ConfigureLoggerCategoryNameInspector(Action<Inspector<string>> action)
        {
            action(LoggerCategoryNameInspector);
            return this;
        }

        /// <summary>
        /// Configures the server log inspector.
        /// </summary>
        /// <param name="action">The action.</param>
        public ServerConfigurator ConfigureServerLogInspector(Action<Inspector<ServerLogEvent>> action)
        {
            action(ServerLogInspector);
            return this;
        }

        /// <summary>
        /// Configures the test services.
        /// </summary>
        /// <param name="configureTestServicesAction">The configure test services action.</param>
        public ServerConfigurator ConfigureTestServices(Action<IServiceCollection> configureTestServicesAction)
        {
            ServiceConfigurators.Add(configureTestServicesAction);
            return this;
        }

        /// <summary>
        /// Sets the minimum log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        public ServerConfigurator SetMinimumLogLevel(LogLevel logLevel)
        {
            MinimumLogLevel = logLevel;
            return this;
        }
    }
}
