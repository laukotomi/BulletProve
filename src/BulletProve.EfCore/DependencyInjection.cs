using BulletProve.EfCore.Hooks;
using BulletProve.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds clean database hook for a DbContext.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public static IServiceCollection AddCleanDatabaseHook<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddScoped<IBeforeTestHook, CleanDatabaseHook<TDbContext>>();

            return services;
        }
    }
}