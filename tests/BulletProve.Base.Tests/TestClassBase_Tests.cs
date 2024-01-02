using BulletProve.Base.Configuration;
using BulletProve.Exceptions;
using FluentAssertions;
using NSubstitute;

namespace BulletProve.Base.Tests
{
    /// <summary>
    /// The test class base tests.
    /// </summary>
    public class TestClassBase_Tests
    {
        /// <summary>
        /// The server name.
        /// </summary>
        private const string ServerName = "server";
        private readonly IOutput _output;
        private readonly ITestServer _server;
        private readonly TestClass _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassBase_Tests"/> class.
        /// </summary>
        public TestClassBase_Tests()
        {
            _output = Substitute.For<IOutput>();
            _server = Substitute.For<ITestServer>();
            var serverFactory = Substitute.For<ITestServerFactory>();
            serverFactory
                .CreateTestServer<ITestStartup>(Arg.Any<Action<ServerConfigurator>>())
                .Returns(_server);

            var manager = new ServerManager(serverFactory);
            _sut = new TestClass(manager, _output);
        }

        /// <summary>
        /// Tests the get server async no servers.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetServerAsyncNoServers()
        {
            var act = () => _sut.GetServerAsync(ServerName);
            await act.Should().ThrowAsync<BulletProveException>();
        }

        /// <summary>
        /// Tests the get server async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetServerAsync()
        {
            _sut.RegisterServers();
            await _sut.GetServerAsync(ServerName);

            await _server.Received(1).StartSessionAsync(ServerName);
        }

        /// <summary>
        /// Tests the end sessions async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestEndSessionsAsync()
        {
            _sut.RegisterServers();
            await _sut.GetServerAsync(ServerName);
            await _sut.EndSessionsAsync();
            await _server.Received(1).EndSessionAsync(_output);
        }

        /// <summary>
        /// Tests the register servers.
        /// </summary>
        [Fact]
        public void TestRegisterServers()
        {
            _sut.RegisterServers();
            _sut.RegisterServers();
            _sut.RegisterServersCalled.Should().Be(1);
        }

        /// <summary>
        /// The test class.
        /// </summary>
        private class TestClass(ServerManager serverManager, IOutput output) : TestClassBase(serverManager, output)
        {
            /// <summary>
            /// Gets a value indicating whether register servers called.
            /// </summary>
            public int RegisterServersCalled { get; private set; }

            /// <inheritdoc/>
            protected override void RegisterServers(IServerRegistrator serverRegistrator)
            {
                RegisterServersCalled++;
                serverRegistrator.RegisterServer<ITestStartup>(ServerName);
            }
        }

        /// <summary>
        /// The test startup.
        /// </summary>
        private interface ITestStartup { }
    }
}
