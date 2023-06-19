using BulletProve.Exceptions;
using BulletProve.TestServer;

namespace BulletProve
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public sealed class TestServerManager : IServerRegistrator, IDisposable
    {
        private readonly Dictionary<string, ITestServer> _testServers = new();

        /// <summary>
        /// Gets a value indicating whether there are any servers.
        /// </summary>
        public bool HasServers => _testServers.Count > 0;

        /// <inheritdoc />
        public void RegisterServer<TStartup>(string serverName, Action<ServerConfigurator>? configAction = null)
            where TStartup : class
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new BulletProveException($"Server name '{serverName}' is not valid");

            if (_testServers.ContainsKey(serverName))
                throw new BulletProveException($"Server '{serverName}' was already registered");

            var server = new TestServer<TStartup>(configAction);
            _testServers.Add(serverName, server);
        }

        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        public ITestServer GetServer(string serverName)
        {
            if (!_testServers.TryGetValue(serverName, out var server))
                throw new BulletProveException($"Server '{serverName}' cannot be found");

            return server;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var (_, server) in _testServers)
            {
                server.Dispose();
            }
        }
    }
}