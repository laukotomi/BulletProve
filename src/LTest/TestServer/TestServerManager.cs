namespace LTest
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public class TestServerManager : IDisposable
    {
        //private readonly Dictionary<Type, ITestServer> _servers = new();
        private readonly Dictionary<string, ITestServer> _testServers = new();
        //private Type? _defaultServer;

        /// <summary>
        /// Gets the test server.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="testSuite">The test suite.</param>
        /// <param name="created">Whether the server just created.</param>
        /// <returns>An ITestServer.</returns>
        //public ITestServer GetServer(ITest testContext, Type testSuite, out bool created)
        //{
        //    created = false;
        //    var serverType = XunitReflectionHelper.TryGetTestServerType(testContext) ?? TryGetDefaultTestServer(testSuite.Assembly);

        //    ValidateServerType(testContext, testSuite, serverType);

        //    if (!_servers.TryGetValue(serverType!, out var server))
        //    {
        //        server = Activator.CreateInstance(serverType) as ITestServer;
        //        _servers.Add(serverType, server!);
        //        created = true;
        //    }

        //    return server!;
        //}

        public void RegisterServer<TServer>(string serverName)
            where TServer : class, ITestServer, new()
        {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new InvalidOperationException($"Server name '{serverName}' is not valid");

            if (_testServers.ContainsKey(serverName))
                throw new InvalidOperationException($"Server '{serverName}' was already registered");

            var server = new TServer();
            _testServers.Add(serverName, server);
        }

        public ITestServer GetServer(string serverName)
        {
            if (!_testServers.TryGetValue(serverName, out var server))
                throw new InvalidOperationException($"Server '{serverName}' cannot be found");

            return server;
        }

        public bool HasServers => _testServers.Count > 0;

        //private static void ValidateServerType(ITest testContext, Type testSuite, Type? serverType)
        //{
        //    if (serverType == null)
        //    {
        //        throw new InvalidOperationException($"Could not find test server for {testSuite.Name}/{testContext.DisplayName}");
        //    }

        //    if (!typeof(ITestServer).IsAssignableFrom(serverType))
        //    {
        //        throw new ArgumentException($"{serverType.Name} does not implement {nameof(ITestServer)}");
        //    }

        //    if (serverType.GetConstructor(Type.EmptyTypes) == null)
        //    {
        //        throw new ArgumentException($"{serverType.Name} should contain parameterless constructor");
        //    }
        //}

        /// <summary>
        /// Tries to get the default test server using the <see cref="DefaultTestServerAttribute"/>.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        //private Type? TryGetDefaultTestServer(Assembly assembly)
        //{
        //    if (_defaultServer == null)
        //    {
        //        var defaultServers = assembly.GetTypes()
        //            .Where(x => x.GetCustomAttribute<DefaultTestServerAttribute>() != null)
        //            .ToList();

        //        if (defaultServers.Count == 1)
        //        {
        //            _defaultServer = defaultServers[0];
        //        }
        //        else if (defaultServers.Count > 1)
        //        {
        //            throw new InvalidOperationException($"More than one default servers found! Default servers: {string.Join(", ", defaultServers.Select(x => x.Name))}");
        //        }
        //    }

        //    return _defaultServer;
        //}

        public void Dispose()
        {
            foreach (var (_, server) in _testServers)
            {
                server.Dispose();
            }
        }
    }
}