using LTest.EFCore.Behaviors;
using LTest.EFCore.Services;
using LTest.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for EFCore testing.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public static IServiceCollection AddTestEFCore(this IServiceCollection services)
        {
            services.AddSingleton<TopologicalSortService>();
            services.AddSingleton<DatabaseCleanupService>();

            return services;
        }

        /// <summary>
        /// Adds clean database behavior for a DbContext.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public static IServiceCollection AddCleanDatabaseBehavior<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddSingleton<IBeforeTestHook, CleanDatabaseBehavior<TDbContext>>();

            return services;
        }
    }
}