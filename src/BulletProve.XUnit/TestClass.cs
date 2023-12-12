using Xunit;
using Xunit.Abstractions;

namespace BulletProve
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    [Collection("Integration Tests")]
    public abstract class TestClass : TestClassBase, IAsyncLifetime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected TestClass(ServerManager serverManager, ITestOutputHelper output) :
            base(serverManager, new OutputAdapter(output))
        {
        }

        /// <inheritdoc/>
        public virtual Task InitializeAsync()
        {
            RegisterServers();
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task DisposeAsync()
        {
            await EndSessionsAsync();
        }
    }
}