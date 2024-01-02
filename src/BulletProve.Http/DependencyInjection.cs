using BulletProve.Base.Configuration;
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
        /// Adds the BulletProve.HTTP services.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="configAction">The config action.</param>
        /// <returns>An IServerConfigurator.</returns>
        public static IServerConfigurator AddBulletProveHttp(this IServerConfigurator configurator, Action<HttpConfiguration>? configAction = null)
        {
            var config = new HttpConfiguration();
            configAction?.Invoke(config);

            configurator.ConfigureTestServices(services =>
            {
                services.AddSingleton(config);
                services.AddSingleton<HttpMethodService>();
                services.AddSingleton<ILinkGeneratorService, LinkGeneratorService>();
                services.AddSingleton<IHttpRequestManager, HttpRequestManager>();
                services.AddScoped<LabelGeneratorService>();
                services.AddSingleton<IServerLogHandler, HttpRequestManager>(sp => (HttpRequestManager)sp.GetRequiredService<IHttpRequestManager>());

                services.AddTransient<IStartupFilter, HttpRequestFilter>();
                services.AddTransient(typeof(IAssertionBuilder<>), typeof(AssertionBuilder<>));
            });

            return configurator;
        }
    }
}