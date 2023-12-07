using BulletProve.EfCore.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;

namespace BulletProve.EfCore.Tests.Services
{
    /// <summary>
    /// The database cleanup service tests.
    /// </summary>
    public class DatabaseCleanupService_Tests
    {
        private readonly DbContextOptions<Context> _contextOptions;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly DatabaseCleanupService _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCleanupService_Tests"/> class.
        /// </summary>
        public DatabaseCleanupService_Tests()
        {
            _contextOptions = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase("Test")
                .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _sqlExecutor = Substitute.For<ISqlExecutor>();
            _sut = new DatabaseCleanupService(_sqlExecutor);
        }

        /// <summary>
        /// Tests the cleanup async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanupAsync()
        {
            using var context = new Context(_contextOptions);
            var sql = string.Empty;
            await _sqlExecutor.ExecuteAsync(context, Arg.Do<string>(s => sql = s));

            await _sut.CleanupAsync(context);

            await _sqlExecutor.Received(1).ExecuteAsync(context, sql);
            sql.Should().StartWith("DELETE FROM \"SubEntity\"");
            sql.Should().Contain("DELETE FROM \"schema\".\"table\"");
        }

        /// <summary>
        /// The context.
        /// </summary>
        private class Context(DbContextOptions options) : DbContext(options)
        {
            /// <summary>
            /// Gets or sets the entities.
            /// </summary>
            public DbSet<Entity> Entities { get; set; }

            /// <inheritdoc/>
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Entity>()
                    .ToTable("table", "schema");
            }
        }

        /// <summary>
        /// The entity.
        /// </summary>
        private class Entity
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public required string Name { get; set; } = null!;

            /// <summary>
            /// Gets or sets the sub entities.
            /// </summary>
            public virtual ICollection<SubEntity> SubEntities { get; set; } = null!;
        }

        /// <summary>
        /// The sub entity.
        /// </summary>
        private class SubEntity
        {
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; } = null!;

            /// <summary>
            /// Gets or sets the entity.
            /// </summary>
            public virtual Entity Entity { get; set; } = null!;
        }
    }
}
