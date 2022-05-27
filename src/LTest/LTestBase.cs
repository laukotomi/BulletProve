using LTest.Exceptions;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly AsyncServiceScope _serviceScope;

        /// <summary>
        /// Access services using this property.
        /// </summary>
        protected LTestFacade LTestServices { get; }

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

            _serviceScope = server.Services.CreateAsyncScope();
            LTestServices = _serviceScope.ServiceProvider.GetRequiredService<LTestFacade>();
            Logger = LTestServices.Logger;
        }

        /// <summary>
        /// Initializes the framework.
        /// </summary>
        /// <returns>A Task.</returns>
        public virtual async Task InitializeAsync()
        {
            if (_serverStarted)
            {
                await HookHelper.RunHooksAsync<IAfterServerStartedHook>(LTestServices, x => x.AfterServerStartedAsync());
            }
            else
            {
                await HookHelper.RunHooksAsync<IResetSingletonHook>(LTestServices, x => x.ResetAsync());
            }

            await HookHelper.RunHooksAsync<IBeforeTestHook>(LTestServices, x => x.BeforeTestAsync());
        }

        /// <summary>
        /// Disposes the framework.
        /// </summary>
        /// <returns>A Task.</returns>
        public virtual async Task DisposeAsync()
        {
            try
            {
                await HookHelper.RunHooksAsync<IAfterTestHook>(LTestServices, x => x.AfterTestAsync());

                FlushLogger();

                if (LTestServices.LogSniffer.UnexpectedLogOccured)
                {
                    throw new LogSnifferException("Unexpected log occured on server side. Check the logs!");
                }
            }
            finally
            {
                await _serviceScope.DisposeAsync();
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