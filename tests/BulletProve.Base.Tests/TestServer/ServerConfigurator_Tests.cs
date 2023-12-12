using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.TestServer
{
    /// <summary>
    /// The server configurator tests.
    /// </summary>
    public class ServerConfigurator_Tests
    {
        private readonly ServerConfigurator _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerConfigurator_Tests"/> class.
        /// </summary>
        public ServerConfigurator_Tests()
        {
            _sut = new ServerConfigurator();
        }

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            _sut.AppSettings.Should().NotBeNull().And.HaveCount(0);
            _sut.HttpClientOptions.BaseAddress.ToString().Should().Be("https://localhost/");
            _sut.JsonConfigurationFiles.Should().NotBeNull().And.HaveCount(0);
            _sut.LoggerCategoryNameInspector.Should().NotBeNull();
            _sut.MinimumLogLevel.Should().Be(LogLevel.Information);
            _sut.ServerLogInspector.Should().NotBeNull();
            _sut.ServiceConfigurators.Should().NotBeNull().And.HaveCount(0);
        }

        /// <summary>
        /// Tests the add app setting.
        /// </summary>
        [Fact]
        public void TestAddAppSetting()
        {
            _sut.AddAppSetting("key", "value");

            _sut.AppSettings.Should().HaveCount(1);
            _sut.AppSettings.Should().ContainKey("key");
            _sut.AppSettings["key"].Should().Be("value");
        }

        /// <summary>
        /// Tests the add json configuration file.
        /// </summary>
        [Fact]
        public void TestAddJsonConfigurationFile()
        {
            _sut.AddJsonConfigurationFile("file.json");

            _sut.JsonConfigurationFiles.Should().HaveCount(1);
            _sut.JsonConfigurationFiles[0].Should().Be("file.json");
        }

        /// <summary>
        /// Tests the configure server log inspector.
        /// </summary>
        [Fact]
        public void TestConfigureServerLogInspector()
        {
            _sut.ConfigureServerLogInspector(x => x.AddAllowedAction(le => le.CategoryName == "aaa"));

            var isAllowed = _sut.ServerLogInspector.IsAllowed(new BulletProve.ServerLog.ServerLogEvent("aaa", LogLevel.Information, new EventId(), string.Empty, null, null));
            isAllowed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the configure test services.
        /// </summary>
        [Fact]
        public void TestConfigureTestServices()
        {
            _sut.ConfigureTestServices(x => x.AddSingleton<string>());

            _sut.ServiceConfigurators.Should().HaveCount(1);
        }

        /// <summary>
        /// Tests the configure logger category name inspector.
        /// </summary>
        [Fact]
        public void TestConfigureLoggerCategoryNameInspector()
        {
            _sut.ConfigureLoggerCategoryNameInspector(x => x.AddAllowedAction(cn => cn == "aaa"));

            var isAllowed = _sut.LoggerCategoryNameInspector.IsAllowed("aaa");
            isAllowed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the set minimum log level.
        /// </summary>
        [Fact]
        public void TestSetMinimumLogLevel()
        {
            _sut.SetMinimumLogLevel(LogLevel.Error);
            _sut.MinimumLogLevel.Should().Be(LogLevel.Error);
        }

        /// <summary>
        /// Tests the configure http client.
        /// </summary>
        [Fact]
        public void TestConfigureHttpClient()
        {
            _sut.ConfigureHttpClient(x => x.BaseAddress = new Uri("https://123.com/"));
            _sut.HttpClientOptions.BaseAddress.ToString().Should().Be("https://123.com/");
        }
    }
}
