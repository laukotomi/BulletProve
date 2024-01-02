using BulletProve.Base.Configuration;
using BulletProve.Base.TestServer;
using BulletProve.Exceptions;

namespace BulletProve
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public sealed class ServerManager : IServerRegistrator, IDisposable
    {
        private readonly List<ITestServer> _testServers = new();
        private readonly Dictionary<string, ITestServer> _testServerMap = new();
        private readonly ITestServerFactory _testServerFactory;

        /// <inheritdoc />
        public bool HasServers => _testServers.Count > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class.
        /// </summary>
        public ServerManager()
        {
            _testServerFactory = new TestServerFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerManager"/> class. Used for unit tests.
        /// </summary>
        /// <param name="testServerFactory">The test server factory.</param>
        internal ServerManager(ITestServerFactory testServerFactory)
        {
            _testServerFactory = testServerFactory;
        }

        /// <inheritdoc />
        public void RegisterServer<TStartup>(string serverName, Action<IServerConfigurator>? configAction = null)
            where TStartup : class
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new BulletProveException($"Server name '{serverName}' is not valid");

            if (_testServerMap.ContainsKey(serverName))
                throw new BulletProveException($"Server '{serverName}' was already registered");

            var server = _testServerFactory.CreateTestServer<TStartup>(configAction);
            _testServers.Add(server);
            _testServerMap.Add(serverName, server);
        }

        /// <inheritdoc />
        public ITestServer GetServer(string serverName)
        {
            if (!_testServerMap.TryGetValue(serverName, out var server))
                throw new BulletProveException($"Server '{serverName}' cannot be found");

            return server;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _testServers.ForEach(x => x.Dispose());
            GC.SuppressFinalize(this);
        }
    }
}