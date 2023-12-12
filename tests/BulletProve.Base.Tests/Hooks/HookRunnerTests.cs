using BulletProve.Hooks;
using BulletProve.Logging;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace BulletProve.Tests.Hooks
{
    /// <summary>
    /// The hook runner tests.
    /// </summary>
    public class HookRunnerTests
    {
        /// <summary>
        /// Tests the run hooks method.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestRunHooksMethod()
        {
            var hooks = new List<TestHook> { new(), new() };

            var collection = new ServiceCollection();
            collection.AddTransient(sp => hooks[0]);
            collection.AddTransient(sp => hooks[1]);
            var sp = collection.BuildServiceProvider();

            var sut = new HookRunner(sp, Substitute.For<ITestLogger>());
            await sut.RunHooksAsync<TestHook>(x => x.RunMethod());

            hooks.Should().AllSatisfy(x => x.Run.Should().BeTrue());
        }

        /// <summary>
        /// The test hook.
        /// </summary>
        private class TestHook : IHook
        {
            /// <summary>
            /// Gets a value indicating whether run.
            /// </summary>
            public bool Run { get; private set; }

            /// <summary>
            /// Runs the method.
            /// </summary>
            /// <returns>A Task.</returns>
            public Task RunMethod()
            {
                Run = true;
                return Task.CompletedTask;
            }
        }
    }
}
