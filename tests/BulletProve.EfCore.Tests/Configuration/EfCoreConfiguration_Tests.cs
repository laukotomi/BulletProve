using BulletProve.EfCore.Configuration;
using BulletProve.EfCore.Services;
using BulletProve.Logging;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.EfCore.Tests.Configuration
{
    /// <summary>
    /// The ef core configuration_ tests.
    /// </summary>
    public class EfCoreConfiguration_Tests
    {
        private readonly ServiceCollection _services;
        private readonly EfCoreConfiguration _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreConfiguration_Tests"/> class.
        /// </summary>
        public EfCoreConfiguration_Tests()
        {
            _services = new ServiceCollection();
            _sut = new EfCoreConfiguration(_services);
        }

        /// <summary>
        /// Tests the clean database method.
        /// </summary>
        [Fact]
        public void TestCleanDatabase1()
        {
            _sut.CleanDatabase<Context>();
            _services.Count.Should().Be(1);
        }

        /// <summary>
        /// Tests the clean database method.
        /// </summary>
        [Fact]
        public void TestCleanDatabase2()
        {
            _sut.CleanDatabase<Context, Seeder>();
            _services.Count.Should().Be(2);
        }

        /// <summary>
        /// The context.
        /// </summary>
        private class Context : DbContext
        {

        }

        /// <summary>
        /// The seeder.
        /// </summary>
        private class Seeder(ITestLogger logger) : DatabaseSeeder(logger)
        {
            /// <inheritdoc/>
            protected override Task SeedAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}
