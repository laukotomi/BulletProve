namespace BulletProve
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        /// <summary>
        /// Inits the test session.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        Task<ServerScope> StartSessionAsync(string serverName);

        /// <summary>
        /// Ends the test session.
        /// </summary>
        /// <param name="output">The output for the logs.</param>
        Task EndSessionAsync(IOutput output);

        /// <summary>
        /// Gets a value indicating whether is disposed.
        /// </summary>
        bool IsDisposed { get; }
    }
}