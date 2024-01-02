using BulletProve.Base.Configuration;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;

namespace BulletProve
{
    /// <summary>
    /// The server scope.
    /// </summary>
    public interface IServerScope : IServiceProvider
    {
        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        ServerConfigurator Configuration { get; }

        /// <summary>
        /// Gets the disposable collertor.
        /// </summary>
        DisposableCollector DisposableCollector { get; }

        /// <summary>
        /// Gets the http client.
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Gets logger service.
        /// </summary>
        ITestLogger Logger { get; }

        /// <summary>
        /// Gets the server log collector.
        /// </summary>
        IServerLogCollector ServerLogCollector { get; }
    }
}