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
            services.AddSingleton<ILinkGeneratorService, LinkGeneratorService>();
            services.AddSingleton<IHttpRequestManager, HttpRequestManager>();
            services.AddScoped<LabelGeneratorService>();
            services.AddSingleton<IServerLogHandler, HttpRequestManager>(sp => (HttpRequestManager)sp.GetRequiredService<IHttpRequestManager>());

            services.AddTransient<IStartupFilter, HttpRequestFilter>();
            services.AddTransient(typeof(IAssertionBuilder<>), typeof(AssertionBuilder<>));

            return services;
        }
    }
}