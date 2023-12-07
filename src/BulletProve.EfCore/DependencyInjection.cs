using BulletProve.EfCore.Hooks;
using BulletProve.EfCore.Services;
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
        private static bool _servicesRegistered;

        /// <summary>
        /// Adds clean database hook for a DbContext.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        public static IServiceCollection AddCleanDatabaseHook<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            RegisterServices(services);
            services.AddScoped<IBeforeTestHook, CleanDatabaseHook<TDbContext>>();

            return services;
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void RegisterServices(IServiceCollection services)
        {
            if (_servicesRegistered)
                return;

            services.AddSingleton<IDatabaseCleanupService, DatabaseCleanupService>();
            services.AddSingleton<ISqlExecutor, SqlExecutor>();
            _servicesRegistered = true;
        }
    }
}