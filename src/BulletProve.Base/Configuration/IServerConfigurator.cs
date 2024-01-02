using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Base.Configuration
{
    /// <summary>
    /// The server configurator.
    /// </summary>
    public interface IServerConfigurator
    {
        /// <summary>
        /// Configures the http client.
        /// </summary>
        /// <param name="action">The action.</param>
        IServerConfigurator ConfigureHttpClient(Action<WebApplicationFactoryClientOptions> action);

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="action">The action.</param>
        IServerConfigurator ConfigureLogger(Action<ILoggerConfigurator> action);

        /// <summary>
        /// Configures the test services.
        /// </summary>
        /// <param name="configureTestServicesAction">The configure test services action.</param>
        IServerConfigurator ConfigureTestServices(Action<IServiceCollection> configureTestServicesAction);

        /// <summary>
        /// Adds the setting.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        IServerConfigurator UseAppSetting(string key, string value);

        /// <summary>
        /// Adds a json configuration file.
        /// </summary>
        /// <param name="file">The file.</param>
        IServerConfigurator UseJsonConfigurationFile(string file);
    }
}
