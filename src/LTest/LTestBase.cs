using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
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
        private readonly ITestOutputHelper _output;
        private readonly bool _serverStarted;

        /// <summary>
        /// Access services using this property.
        /// </summary>
        protected IntegrationTestServiceProvider Services { get; }

        /// <summary>
        /// Test context.
        /// </summary>
        protected ITest TestContext { get; }

        /// <summary>
        /// Test logger.
        /// </summary>
        protected ITestLogger Logger { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestBase"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected LTestBase(TestServerManager serverManager, ITestOutputHelper output)
        {
            _output = output;
            TestContext = XunitReflectionHelper.GetTestContext(output);

            var server = serverManager.GetServer(TestContext, GetType(), out _serverStarted);

            Services = new IntegrationTestServiceProvider(server.Services);
            Logger = Services.Logger;
        }

        /// <summary>
        /// Initializes the framework.
        /// </summary>
        /// <returns>A Task.</returns>
        public virtual async Task InitializeAsync()
        {
            if (_serverStarted)
            {
                await HookHelper.RunHooksAsync<IAfterServerStartedHook>(Services, x => x.AfterServerStartedAsync());
            }
            else
            {
                await HookHelper.RunHooksAsync<IResetSingletonHook>(Services, x => x.ResetAsync());
            }

            await HookHelper.RunHooksAsync<IBeforeTestHook>(Services, x => x.BeforeTestAsync());
        }

        /// <summary>
        /// Disposes the framework.
        /// </summary>
        /// <returns>A Task.</returns>
        public virtual async Task DisposeAsync()
        {
            await HookHelper.RunHooksAsync<IAfterTestHook>(Services, x => x.AfterTestAsync());

            FlushLogger();

            if (Services.LogSniffer.UnexpectedLogOccured)
            {
                throw new InvalidOperationException($"Unexpected log occured on server side while sending the request. Check the logs!");
            }
        }

        /// <summary>
        /// Flushes the logger.
        /// </summary>
        private void FlushLogger()
        {
            Logger.LogInformation("Test finished");

            var logger = (TestLogger)Logger;
            var logEvents = logger.GetSnapshot();
            foreach (var logEvent in logEvents)
            {
                _output.WriteLine(logEvent.ToString());
            }
        }
    }
}