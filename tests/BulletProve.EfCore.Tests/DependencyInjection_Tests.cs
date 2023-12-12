using BulletProve.EfCore.Services;
using BulletProve.Hooks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.EfCore.Tests
{
    /// <summary>
    /// The dependency injection tests.
    /// </summary>
    public class DependencyInjection_Tests
    {
        /// <summary>
        /// Tests the add clean database hook.
        /// </summary>
        [Fact]
        public void TestAddCleanDatabaseHook()
        {
            var services = new ServiceCollection();
            DependencyInjection.AddCleanDatabaseHook<Context>(services);
            DependencyInjection.AddCleanDatabaseHook<Context>(services);

            services.Count(x => x.ServiceType == typeof(IDatabaseCleanupService)).Should().Be(1);
            services.Count(x => x.ServiceType == typeof(ISqlExecutor)).Should().Be(1);
            services.Count(x => x.ServiceType == typeof(IBeforeTestHook)).Should().Be(2);
        }

        /// <summary>
        /// The context.
        /// </summary>
        private class Context : DbContext
        {

        }
    }
}
