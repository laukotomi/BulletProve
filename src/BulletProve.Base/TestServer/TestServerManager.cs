using BulletProve.Base.TestServer;
using BulletProve.Exceptions;
using BulletProve.TestServer;

namespace BulletProve
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public sealed class TestServerManager : IServerManager, IDisposable
    {
        private readonly Dictionary<string, ITestServer> _testServers = new();

        /// <inheritdoc />
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

        /// <inheritdoc />
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
#pragma warning disable S3966 // Objects should not be disposed more than once
                server.Dispose();
#pragma warning restore S3966 // Objects should not be disposed more than once
            }
        }
    }
}