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
        /// Access services using this property.
        /// </summary>
        //protected LTestFacade Test { get; }

        /// <summary>
        /// Test context.
        /// </summary>
        //protected ITest TestContext { get; }

        /// <summary>
        /// Test logger.
        /// </summary>
        //protected ITestLogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestBase"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected LTestBase(TestServerManager serverManager, ITestOutputHelper output)
        {
            _serverManager = serverManager;
            _output = output;
            //TestContext = XunitReflectionHelper.GetTestContext(output);

            //var server = serverManager.GetServer(TestContext, GetType(), out _serverStarted);

            //_serviceScope = server.Services.CreateAsyncScope();
            //Test = _serviceScope.ServiceProvider.GetRequiredService<LTestFacade>();
            //Logger = Test.Logger;
        }

        public abstract void RegisterServers(ServerRegistrator serverRegistrator);

        public async Task<LTestFacade> GetServerAsync(string serverName)
        {
            var server = _serverManager.GetServer(serverName);
            _usedServers.Add(server);

            var facade = await server.InitScopeAsync(serverName);

            return facade;
        }

        public virtual Task InitializeAsync()
        {
            if (!_serverManager.HasServers)
            {
                RegisterServers(new ServerRegistrator(_serverManager));
            }

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