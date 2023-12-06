using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Base.Tests
{
    /// <summary>
    /// The server scope tests.
    /// </summary>
    public class ServerScope_Tests
    {
        private readonly ServiceProvider _provider;
        private readonly HttpClient _httpClient;
        private readonly ServerScope _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerScope_Tests"/> class.
        /// </summary>
        public ServerScope_Tests()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ServerConfigurator>();
            services.AddSingleton<IServerLogCollector, ServerLogCollector>();
            services.AddSingleton<ScopeProvider>();
            services.AddSingleton<DisposableCollector>();
            services.AddSingleton<ITestLogger, TestLogger>();

            _provider = services.BuildServiceProvider();

            _httpClient = new HttpClient();
            _sut = new ServerScope(_provider, _httpClient);
        }

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            _sut.DisposableCollector.Should().NotBeNull();
            _sut.Configuration.Should().NotBeNull();
            _sut.HttpClient.Should().Be(_httpClient);
            _sut.Logger.Should().NotBeNull().And.BeOfType<TestLogger>();
            _sut.ServerLogCollector.Should().NotBeNull().And.BeOfType<ServerLogCollector>();
        }

        /// <summary>
        /// Tests the get service.
        /// </summary>
        [Fact]
        public void TestGetService()
        {
            _sut.GetService<ITestLogger>().Should().NotBeNull().And.BeOfType<TestLogger>();
        }

        /// <summary>
        /// Tests the dispose.
        /// </summary>
        [Fact]
        public void TestDispose()
        {
            _sut.Dispose();
            var act = () => _sut.GetService<ITestLogger>();
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the dispose async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestDisposeAsync()
        {
            await _sut.DisposeAsync();
            var act = () => _sut.GetService<ITestLogger>();
            act.Should().Throw<Exception>();
        }
    }
}
