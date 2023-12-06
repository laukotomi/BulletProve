using BulletProve.Base.TestServer;
using BulletProve.Exceptions;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        /// Tests the register servers called.
        /// </summary>
        [Fact]
        public void TestRegisterServersCalled()
        {
            var sut = new EmptyTestClass(new TestServerManager());
            sut.IsRegisterServersCalled.Should().BeTrue();
        }

        /// <summary>
        /// Tests the get server async no servers.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetServerAsyncNoServers()
        {
            var sut = new EmptyTestClass(new TestServerManager());
            var act = () => sut.GetServerAsync("asd");
            await act.Should().ThrowAsync<BulletProveException>();
        }

        /// <summary>
        /// Tests the get server async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestGetServerAsync()
        {
            var manager = new ServerManager();
            var sut = new TestClass(manager);
            var scope = await sut.GetServerAsync(ServerName);
            scope.Should().NotBeNull().And.Be(manager.ServerScope);

            await manager.Server.Received(1).StartSessionAsync(ServerName);
        }

        [Fact]
        public async Task TestEndSessionsAsync()
        {
            var manager = new ServerManager();
            var sut = new TestClass(manager);
            await sut.GetServerAsync(ServerName);
            await sut.EndSessionsAsync(null!);
            await manager.Server.Received(1).EndSessionAsync(Arg.Any<IOutput>());
        }

        /// <summary>
        /// The test class.
        /// </summary>
        private class EmptyTestClass(IServerManager serverManager) : TestClassBase(serverManager)
        {
            /// <summary>
            /// Gets a value indicating whether register servers is called.
            /// </summary>
            public bool IsRegisterServersCalled { get; private set; }

            /// <inheritdoc/>
            public override void RegisterServers(IServerRegistrator serverRegistrator)
            {
                IsRegisterServersCalled = true;
            }
        }

        /// <summary>
        /// The server manager.
        /// </summary>
        private class ServerManager : IServerManager
        {
            private Dictionary<string, ITestServer> _servers = new();

            /// <inheritdoc/>
            public bool HasServers => _servers.Count > 0;

            /// <summary>
            /// Gets the server scope.
            /// </summary>
            public ServerScope ServerScope { get; }

            /// <summary>
            /// Gets the server.
            /// </summary>
            public ITestServer Server { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ServerManager"/> class.
            /// </summary>
            public ServerManager()
            {
                var services = new ServiceCollection();
                services.AddSingleton<ServerConfigurator>();
                services.AddSingleton<IServerLogCollector, ServerLogCollector>();
                services.AddSingleton<ScopeProvider>();
                services.AddSingleton<DisposableCollector>();
                services.AddSingleton<ITestLogger, TestLogger>();

                var provider = services.BuildServiceProvider();

                ServerScope = new ServerScope(provider, null!);
                Server = Substitute.For<ITestServer>();
                Server.StartSessionAsync(ServerName).Returns(ServerScope);
            }

            /// <inheritdoc/>
            public ITestServer GetServer(string serverName)
            {
                return _servers[serverName];
            }

            /// <inheritdoc/>
            public void RegisterServer<TStartup>(string serverName, Action<ServerConfigurator>? configAction = null) where TStartup : class
            {
                _servers.Add(serverName, Server);
            }
        }

        /// <summary>
        /// The test class.
        /// </summary>
        private class TestClass(IServerManager serverManager) : TestClassBase(serverManager)
        {
            /// <inheritdoc/>
            public override void RegisterServers(IServerRegistrator serverRegistrator)
            {
                serverRegistrator.RegisterServer<ITestStartup>(ServerName);
            }
        }

        /// <summary>
        /// The test startup.
        /// </summary>
        private interface ITestStartup { }
    }
}
