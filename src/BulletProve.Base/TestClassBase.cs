using BulletProve.Base.TestServer;
using BulletProve.Exceptions;
using BulletProve.TestServer;

namespace BulletProve
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    public abstract class TestClassBase
    {
        private readonly IServerManager _serverManager;
        private readonly List<ITestServer> _usedServers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassBase"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        protected TestClassBase(IServerManager serverManager)
        {
            _serverManager = serverManager;

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

            var scope = await server.StartSessionAsync(serverName);
            return scope;
        }

        /// <summary>
        /// Ends the sessions async.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <returns>A Task.</returns>
        public async Task EndSessionsAsync(IOutput output)
        {
            foreach (var server in _usedServers)
            {
                await server.EndSessionAsync(output);
            }
        }
    }
}