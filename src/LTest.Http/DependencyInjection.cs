using LTest.Http.Configuration;
using LTest.Http.Filters;
using LTest.Http.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
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
        public static IServiceCollection AddLTestHttp(this IServiceCollection services, Action<HttpConfiguration>? configAction = null)
        {
            var config = new HttpConfiguration();
            configAction?.Invoke(config);

            services.AddSingleton(config);
            services.AddSingleton<HttpMethodService>();
            services.AddSingleton<LinkGeneratorService>();
            services.AddSingleton<HttpRequestManager>();
            services.AddScoped<LabelGeneratorService>();

            services.AddTransient<IStartupFilter, HttpRequestFilter>();

            return services;
        }
    }
}