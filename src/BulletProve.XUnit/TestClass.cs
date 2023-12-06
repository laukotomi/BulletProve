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
        private readonly OutputAdapter _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected TestClass(TestServerManager serverManager, ITestOutputHelper output) :
            base(serverManager)
        {
            _output = new(output);
        }

        /// <inheritdoc/>
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task DisposeAsync()
        {
            await EndSessionsAsync(_output);
        }
    }
}