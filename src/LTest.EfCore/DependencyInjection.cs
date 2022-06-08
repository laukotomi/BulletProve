using LTest.EfCore.Hooks;
using LTest.EfCore.Services;
using LTest.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds services for EFCore testing.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public static IServiceCollection AddLTestEFCore(this IServiceCollection services)
        {
            services.AddSingleton<TopologicalSortService>();
            services.AddSingleton<DatabaseCleanupService>();

            return services;
        }

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