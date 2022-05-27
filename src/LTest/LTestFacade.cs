using LTest.Configuration;
using LTest.Http;
using LTest.Logging;
using LTest.LogSniffer;
using LTest.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
{
    /// <summary>
    /// Service provider extensions for integration tests.
    /// </summary>
    public class LTestFacade : IServiceProvider
    {
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestFacade"/> class.
        /// </summary>
        /// <param name="services">Service provider</param>
        public LTestFacade(IServiceProvider services)
        {
            _services = services;
            DisposableCollertor = _services.GetRequiredService<DisposableCollertor>();
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
        public HttpClient HttpClient => _services.GetRequiredService<LTestHttpClientAccessor>().Client;

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public LTestConfiguration Configuration => _services.GetRequiredService<LTestConfiguration>();

        /// <summary>
        /// Gets the disposable collertor.
        /// </summary>
        public DisposableCollertor DisposableCollertor { get; }

        /// <summary>
        /// Returns the service.
        /// </summary>
        /// <param name="serviceType">Service type.</param>
        public object? GetService(Type serviceType)
        {
            return _services.GetService(serviceType);
        }
    }
}