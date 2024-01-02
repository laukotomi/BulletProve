using BulletProve.Base.Configuration;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Base.Tests.Configuration
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
            _sut.LoggerConfigurator.Should().NotBeNull();
            _sut.ServiceConfigurators.Should().NotBeNull().And.HaveCount(0);
        }

        /// <summary>
        /// Tests the add app setting.
        /// </summary>
        [Fact]
        public void TestAddAppSetting()
        {
            _sut.UseAppSetting("key", "value");

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
            _sut.UseJsonConfigurationFile("file.json");

            _sut.JsonConfigurationFiles.Should().HaveCount(1);
            _sut.JsonConfigurationFiles[0].Should().Be("file.json");
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
