using BulletProve.Exceptions;
using BulletProve.TestServer;
using Xunit;
using Xunit.Abstractions;

namespace BulletProve
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    [Collection("Integration Tests")]
    public abstract class TestClass : IDisposable
    {
        private readonly TestServerManager _serverManager;
        private readonly ITestOutputHelper _output;
        private readonly List<ITestServer> _usedServers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected TestClass(TestServerManager serverManager, ITestOutputHelper output)
        {
            _serverManager = serverManager;
            _output = output;

            if (!_serverManager.HasServers)
            {
                RegisterServers(_serverManager);
            }
        }

        /// <summary>
        /// Registers the servers.
        /// </summary>
        /// <param name="serverRegistrator">The server registrator.</param>
        public abstract void RegisterServers(IServerRegistrator serverRegistrator);

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        public async Task<ServerScope> GetServerAsync(string serverName)
        {
            if (!_serverManager.HasServers)
            {
                throw new BulletProveException($"No servers have been registered. Please register servers in {nameof(RegisterServers)} method.");
            }

            var server = _serverManager.GetServer(serverName);
            _usedServers.Add(server);

            var scope = await server.InitScopeAsync(serverName);
            return scope;
        }

        /// <summary>
        /// Disposes the class.
        /// </summary>
        /// <param name="disposing">If true, disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var server in _usedServers)
                {
                    server.CleanUpAsync(_output).GetAwaiter().GetResult();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}