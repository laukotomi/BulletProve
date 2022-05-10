using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace IntegTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;

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
