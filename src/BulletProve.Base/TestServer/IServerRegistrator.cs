using BulletProve.TestServer;

namespace BulletProve
{
    /// <summary>
    /// The server registrator interface.
    /// </summary>
    public interface IServerRegistrator
    {
        /// <summary>
        /// Registers the server.
        /// </summary>
        /// <typeparam name="TStartup">The startup class.</typeparam>
        /// <param name="serverName">The server name.</param>
        /// <param name="configAction">The config action.</param>
        public void RegisterServer<TStartup>(string serverName, Action<ServerConfigurator>? configAction = null)
            where TStartup : class;
    }
}
