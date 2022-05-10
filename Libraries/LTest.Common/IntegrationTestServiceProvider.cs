using LTest.Configuration;
using LTest.Http;
using LTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace LTest
{
    /// <summary>
    /// Service provider extensions for integration tests.
    /// </summary>
    public class IntegrationTestServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestServiceProvider"/> class.
        /// </summary>
        /// <param name="services">Service provider</param>
        public IntegrationTestServiceProvider(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Gets logger service.
        /// </summary>
        public ITestLogger Logger => _services.GetRequiredService<ITestLogger>();


        /// <summary>
        /// Gets log sniffer service.
        /// </summary>
        public ILogSnifferService LogSniffer => _services.GetRequiredService<ILogSnifferService>();

        /// <summary>
        /// Gets the http client.
        /// </summary>
        public HttpClient HttpClient => _services.GetRequiredService<HttpClientAccessor>().Client;

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public IntegrationTestConfiguration Configuration => _services.GetRequiredService<IntegrationTestConfiguration>();

        /// <summary>
        /// Returns the service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        public object GetService(Type serviceType)
        {
            return _services.GetService(serviceType);
        }
    }
}