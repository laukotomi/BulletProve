using BulletProve.Http.Configuration;
using BulletProve.Http.Filters;
using BulletProve.Http.Services;
using BulletProve.ServerLog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds services for Http testing.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configAction">Configuration action.</param>
        public static IServiceCollection AddTestHttp(this IServiceCollection services, Action<HttpConfiguration>? configAction = null)
        {
            var config = new HttpConfiguration();
            configAction?.Invoke(config);

            services.AddSingleton(config);
            services.AddSingleton<HttpMethodService>();
            services.AddSingleton<LinkGeneratorService>();
            services.AddSingleton<HttpRequestManager>();
            services.AddScoped<LabelGeneratorService>();
            services.AddSingleton<IServerLogHandler, HttpRequestManager>(sp => sp.GetRequiredService<HttpRequestManager>());

            services.AddTransient<IStartupFilter, HttpRequestFilter>();

            return services;
        }
    }
}