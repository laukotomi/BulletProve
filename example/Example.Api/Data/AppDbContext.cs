using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Example.Api.Data
{
    /// <summary>
    /// The app db context.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;
        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        public DbSet<Post> Posts { get; set; } = null!;

        /// <summary>
        /// Starts the update.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>An EntityEntry.</returns>
        public EntityEntry<TEntity> StartUpdate<TEntity>(TEntity entity) where TEntity : class
        {
            var entry = Set<TEntity>().Attach(entity);

            if (!entry.IsKeySet)
            {
                throw new InvalidOperationException("You must set the key property to update the entity");
            }

            return entry;
        }
    }
}
