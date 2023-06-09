using LTest.Logging;
using LTest.LogSniffer;
using LTest.Services;
using LTest.TestServer;
using Microsoft.Extensions.DependencyInjection;

namespace LTest
{
    /// <summary>
    /// Service provider extensions for integration tests.
    /// </summary>
    public class LTestFacade : IServiceProvider, IDisposable, IAsyncDisposable
    {
        private readonly AsyncServiceScope _serviceScope;
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="LTestFacade"/> class.
        /// </summary>
        /// <param name="services">Service provider</param>
        public LTestFacade(IServiceProvider services, HttpClient httpClient)
        {
            _serviceScope = services.CreateAsyncScope();
            _services = _serviceScope.ServiceProvider;

            DisposableCollertor = _services.GetRequiredService<DisposableCollertor>();
            Logger = _services.GetRequiredService<ITestLogger>();
            LogSniffer = _services.GetRequiredService<ILogSnifferService>();
            HttpClient = httpClient;
            Configuration = _services.GetRequiredService<ServerConfigurator>();
        }

        /// <summary>
        /// Gets logger service.
        /// </summary>
        public ITestLogger Logger { get; }

        /// <summary>
        /// Gets log sniffer service.
        /// </summary>
        public ILogSnifferService LogSniffer { get; }

        /// <summary>
        /// Gets the http client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public ServerConfigurator Configuration { get; }

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

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            _serviceScope.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <returns>A ValueTask.</returns>
        public async ValueTask DisposeAsync()
        {
            await _serviceScope.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}