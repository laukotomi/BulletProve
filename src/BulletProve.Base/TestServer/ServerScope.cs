using BulletProve.Base.Configuration;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve
{
    /// <summary>
    /// Service provider extensions for integration tests.
    /// </summary>
    public sealed class ServerScope : IServerScope, IDisposable, IAsyncDisposable
    {
        private readonly AsyncServiceScope _serviceScope;
        private readonly IServiceProvider _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerScope"/> class.
        /// </summary>
        /// <param name="services">Service provider</param>
        public ServerScope(IServiceProvider services, HttpClient httpClient)
        {
            _serviceScope = services.CreateAsyncScope();
            _services = _serviceScope.ServiceProvider;

            DisposableCollector = _services.GetRequiredService<DisposableCollector>();
            Logger = _services.GetRequiredService<ITestLogger>();
            ServerLogCollector = _services.GetRequiredService<IServerLogCollector>();
            HttpClient = httpClient;
            Configuration = _services.GetRequiredService<ServerConfigurator>();
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        public ServerConfigurator Configuration { get; }

        /// <summary>
        /// Gets the disposable collertor.
        /// </summary>
        public DisposableCollector DisposableCollector { get; }

        /// <summary>
        /// Gets the http client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Gets logger service.
        /// </summary>
        public ITestLogger Logger { get; }

        /// <summary>
        /// Gets the server log collector.
        /// </summary>
        public IServerLogCollector ServerLogCollector { get; }

        /// <inheritdoc />
        public object? GetService(Type serviceType)
        {
            return _services.GetService(serviceType);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _serviceScope.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await _serviceScope.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}