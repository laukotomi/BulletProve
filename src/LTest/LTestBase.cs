using LTest.Exceptions;
using LTest.TestServer;
using Xunit;
using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    [Collection("Integration Tests")]
    public abstract class LTestBase : IAsyncLifetime
    {
        private readonly TestServerManager _serverManager;
        private readonly ITestOutputHelper _output;
        private readonly List<ITestServer> _usedServers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestBase"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected LTestBase(TestServerManager serverManager, ITestOutputHelper output)
        {
            _serverManager = serverManager;
            _output = output;

            if (!_serverManager.HasServers)
            {
                RegisterServers(_serverManager);
            }
        }

        public abstract void RegisterServers(IServerRegistrator serverRegistrator);

        public async Task<LTestFacade> GetServerAsync(string serverName)
        {
            if (!_serverManager.HasServers)
            {
                throw new BulletProveException($"No servers have been registered. Please register servers in {nameof(RegisterServers)} method.");
            }

            var server = _serverManager.GetServer(serverName);
            _usedServers.Add(server);

            var facade = await server.InitScopeAsync(serverName);
            return facade;
        }

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the framework.
        /// </summary>
        /// <returns>A Task.</returns>
        public virtual async Task DisposeAsync()
        {
            foreach (var server in _usedServers)
            {
                await server.CleanUpAsync(_output);
            }
        }
    }
}