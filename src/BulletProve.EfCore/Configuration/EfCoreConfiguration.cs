using BulletProve.Base.Hooks;
using BulletProve.EfCore.Hooks;
using BulletProve.EfCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.EfCore.Configuration
{
    /// <summary>
    /// The EfCore configuration.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="EfCoreConfiguration"/> class.
    /// </remarks>
    /// <param name="services">The services.</param>
    public class EfCoreConfiguration(IServiceCollection services)
    {
        /// <summary>
        /// Registers a DbContext to be cleaned before each test.
        /// </summary>
        public EfCoreConfiguration CleanDatabase<TDbContext>()
            where TDbContext : DbContext
        {
            services.AddScoped<IBeforeTestHook, CleanDatabaseHook<TDbContext>>();
            return this;
        }

        /// <summary>
        /// Registers a DbContext to be cleaned and a Seeder class to run before each test.
        /// </summary>
        /// <returns>An EfCoreConfiguration.</returns>
        public EfCoreConfiguration CleanDatabase<TDbContext, TSeeder>()
            where TDbContext : DbContext
            where TSeeder : DatabaseSeeder
        {
            services.AddScoped<IBeforeTestHook, CleanDatabaseHook<TDbContext>>();
            services.AddScoped<IBeforeTestHook, TSeeder>();
            return this;
        }
    }
}
