using BulletProve.TestServer;
using FluentAssertions;
using NSubstitute;
using Xunit.Abstractions;

namespace BulletProve.XUnit.Tests
{
    /// <summary>
    /// The test class_ tests.
    /// </summary>
    public class TestClass_Tests
    {
        /// <summary>
        /// The server name.
        /// </summary>
        private const string ServerName = "server";

        private readonly ITestOutputHelper _output;
        private readonly TestTestClass _sut;
        private readonly ITestServer _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass_Tests"/> class.
        /// </summary>
        public TestClass_Tests()
        {
            _server = Substitute.For<ITestServer>();
            var serverFactory = Substitute.For<ITestServerFactory>();
            serverFactory
                .CreateTestServer<ITestStartup>(Arg.Any<Action<ServerConfigurator>>())
                .Returns(_server);

            _output = Substitute.For<ITestOutputHelper>();
            _sut = new TestTestClass(new ServerManager(serverFactory), _output);
        }

        /// <summary>
        /// Tests the initialize async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestInitializeAsync()
        {
            await _sut.InitializeAsync();
            await _sut.InitializeAsync();
            _sut.RegisterServersCalledNr.Should().Be(1);
        }

        /// <summary>
        /// Tests the dispose async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestDisposeAsync()
        {
            await _sut.InitializeAsync();
            await _sut.GetServerAsync(ServerName);
            await _sut.DisposeAsync();

            await _server!.Received(1).EndSessionAsync(Arg.Any<IOutput>());
        }

        /// <summary>
        /// The test test class.
        /// </summary>
        private class TestTestClass(ServerManager serverManager, ITestOutputHelper output) : TestClass(serverManager, output)
        {
            /// <summary>
            /// Gets the register servers called nr.
            /// </summary>
            public int RegisterServersCalledNr { get; private set; }

            /// <summary>
            /// Registers the servers.
            /// </summary>
            /// <param name="serverRegistrator">The server registrator.</param>
            protected override void RegisterServers(IServerRegistrator serverRegistrator)
            {
                RegisterServersCalledNr++;
                serverRegistrator.RegisterServer<ITestStartup>(ServerName);
            }

        }

        /// <summary>
        /// The startup.
        /// </summary>
        private interface ITestStartup { }
    }
}
