using BulletProve.Exceptions;

namespace BulletProve
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    public abstract class TestClassBase
    {
        private readonly List<ITestServer> _usedServers = new();
        protected readonly ServerManager _serverManager;
        private readonly IOutput _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClassBase"/> class.
        /// </summary>
        /// <param name="serverManager">The server manager.</param>
        protected TestClassBase(ServerManager serverManager, IOutput output)
        {
            _serverManager = serverManager;
            _output = output;
        }

        /// <summary>
        /// Registers the servers.
        /// </summary>
        /// <param name="serverRegistrator">The server registrator.</param>
        protected abstract void RegisterServers(IServerRegistrator serverRegistrator);

        /// <summary>
        /// Starts server registration.
        /// </summary>
        public void RegisterServers()
        {
            if (!_serverManager.HasServers)
            {
                RegisterServers(_serverManager);
            }
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        public async Task<IServerScope> GetServerAsync(string serverName)
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
        public async Task EndSessionsAsync()
        {
            foreach (var server in _usedServers)
            {
                await server.EndSessionAsync(_output);
            }
        }
    }
}