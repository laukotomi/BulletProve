using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using BulletProve.TestServer;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve
{
    /// <summary>
    /// Service provider extensions for integration tests.
    /// </summary>
    public sealed class ServerScope : IServiceProvider, IDisposable, IAsyncDisposable
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

            DisposableCollertor = _services.GetRequiredService<DisposableCollertor>();
            Logger = _services.GetRequiredService<ITestLogger>();
            LogSniffer = _services.GetRequiredService<IServerLogsService>();
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
        public IServerLogsService LogSniffer { get; }

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