using BulletProve.Base.Configuration;
using BulletProve.EfCore.Configuration;
using BulletProve.EfCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the BulletProve.EfCore package.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="configAction">The config action.</param>
        public static IServerConfigurator AddBulletProveEfCore(this IServerConfigurator configurator, Action<EfCoreConfiguration> configAction)
        {
            configurator.ConfigureTestServices(services =>
            {
                services.AddSingleton<IDatabaseCleanupService, DatabaseCleanupService>();
                services.AddSingleton<ISqlExecutor, SqlExecutor>();

                var configuration = new EfCoreConfiguration(services);
                configAction(configuration);
            });

            return configurator;
        }
    }
}