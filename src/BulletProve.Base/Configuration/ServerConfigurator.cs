using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Base.Configuration
{
    /// <summary>
    /// The server configurator.
    /// </summary>
    public class ServerConfigurator : IServerConfigurator
    {
        /// <summary>
        /// Gets the app settings.
        /// </summary>
        public Dictionary<string, string> AppSettings { get; } = [];

        /// <summary>
        /// Gets the http client options.
        /// </summary>
        public WebApplicationFactoryClientOptions HttpClientOptions { get; } = new()
        {
#pragma warning disable S1075 // URIs should not be hardcoded
            BaseAddress = new Uri("https://localhost/")
#pragma warning restore S1075 // URIs should not be hardcoded
        };

        /// <summary>
        /// Gets the json configuration files.
        /// </summary>
        public List<string> JsonConfigurationFiles { get; } = [];

        /// <summary>
        /// Gets the logger configurator.
        /// </summary>
        public LoggerConfigurator LoggerConfigurator { get; } = new();

        /// <summary>
        /// Gets the service configurators.
        /// </summary>
        public List<Action<IServiceCollection>> ServiceConfigurators { get; } = [];

        /// <inheritdoc/>
        public IServerConfigurator ConfigureHttpClient(Action<WebApplicationFactoryClientOptions> action)
        {
            action(HttpClientOptions);
            return this;
        }

        /// <inheritdoc/>
        public IServerConfigurator ConfigureLogger(Action<ILoggerConfigurator> action)
        {
            action(LoggerConfigurator);
            return this;
        }

        /// <inheritdoc/>
        public IServerConfigurator ConfigureTestServices(Action<IServiceCollection> configureTestServicesAction)
        {
            ServiceConfigurators.Add(configureTestServicesAction);
            return this;
        }

        /// <inheritdoc/>
        public IServerConfigurator UseAppSetting(string key, string value)
        {
            AppSettings.Add(key, value);
            return this;
        }

        /// <inheritdoc/>
        public IServerConfigurator UseJsonConfigurationFile(string file)
        {
            JsonConfigurationFiles.Add(file);
            return this;
        }
    }
}
