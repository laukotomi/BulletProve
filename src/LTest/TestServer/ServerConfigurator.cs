using LTest.ServerLog;
using LTest.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LTest.TestServer
{
    public class ServerConfigurator
    {
        public Dictionary<string, string> Settings { get; } = new();
        public List<string> JsonConfigurationFiles { get; } = new();
        public List<Action<IServiceCollection>> ServiceConfigurators { get; } = new();
        public Inspector<string> LoggerCategoryNameInspector { get; } = new();
        public Inspector<ServerLogEvent> ServerLogInspector { get; } = new();
        public WebApplicationFactoryClientOptions HttpClientOptions { get; } = new()
        {
            BaseAddress = new Uri("https://localhost")
        };
        public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

        public ServerConfigurator AddSetting(string key, string value)
        {
            Settings.Add(key, value);
            return this;
        }

        public ServerConfigurator AddJsonConfigurationFile(string file)
        {
            JsonConfigurationFiles.Add(file);
            return this;
        }

        public ServerConfigurator ConfigureServerLogInspector(Action<Inspector<ServerLogEvent>> action)
        {
            action(ServerLogInspector);
            return this;
        }

        public ServerConfigurator ConfigureTestServices(Action<IServiceCollection> configureTestServicesAction)
        {
            ServiceConfigurators.Add(configureTestServicesAction);
            return this;
        }

        public ServerConfigurator ConfigureLoggerCategoryNameInspector(Action<Inspector<string>> action)
        {
            action(LoggerCategoryNameInspector);
            return this;
        }

        public ServerConfigurator SetMinimumLogLevel(LogLevel logLevel)
        {
            MinimumLogLevel = logLevel;
            return this;
        }

        public ServerConfigurator ConfigureHttpClient(Action<WebApplicationFactoryClientOptions> action)
        {
            action(HttpClientOptions);
            return this;
        }
    }
}
