using BulletProve.TestServer;

namespace BulletProve.Base.TestServer
{
    /// <summary>
    /// The server manager.
    /// </summary>
    public interface IServerManager : IServerRegistrator
    {
        /// <summary>
        /// Gets the server.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        ITestServer GetServer(string serverName);

        /// <summary>
        /// Gets a value indicating whether there are any servers.
        /// </summary>
        public bool HasServers { get; }
    }
}
