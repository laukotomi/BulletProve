using LTest.TestServer;

namespace LTest
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public class TestServerManager : IServerRegistrator, IDisposable
    {
        private readonly Dictionary<string, ITestServer> _testServers = new();

        public bool HasServers => _testServers.Count > 0;

        public void RegisterServer<TStartup>(string serverName, Action<ServerConfigurator>? configAction = null)
            where TStartup : class
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new InvalidOperationException($"Server name '{serverName}' is not valid");

            if (_testServers.ContainsKey(serverName))
                throw new InvalidOperationException($"Server '{serverName}' was already registered");

            var server = new TestServer<TStartup>(configAction);
            _testServers.Add(serverName, server);
        }

        public ITestServer GetServer(string serverName)
        {
            if (!_testServers.TryGetValue(serverName, out var server))
                throw new InvalidOperationException($"Server '{serverName}' cannot be found");

            return server;
        }

        public void Dispose()
        {
            foreach (var (_, server) in _testServers)
            {
                server.Dispose();
            }
        }
    }
}