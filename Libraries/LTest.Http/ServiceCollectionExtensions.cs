using LTest.Http.Configuration;
using LTest.Http.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LTest
{
    /// <summary>
    /// ServiceCollectionExtensions.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for Http testing.
        /// </summary>
        /// <param name="services">IServiceCollection.</param>
        /// <param name="configAction">Configuration action.</param>
        public static IServiceCollection AddTestHttp(this IServiceCollection services, Action<HttpConfigurationBuilder> configAction = null)
        {
            var configBuilder = new HttpConfigurationBuilder();
            configAction?.Invoke(configBuilder);
            var config = configBuilder.Build();

            services.AddSingleton(config);
            services.AddSingleton<HttpMethodService>();
            services.AddSingleton<HttpRequestBuilder>();
            services.AddSingleton(typeof(HttpRequestBuilder<>));
            services.AddSingleton<LinkGeneratorService>();

            return services;
        }
    }
}