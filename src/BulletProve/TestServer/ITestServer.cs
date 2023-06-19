using Xunit.Abstractions;

namespace BulletProve
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        /// <summary>
        /// Inits the server scope.
        /// </summary>
        /// <param name="serverName">The server name.</param>
        Task<ServerScope> InitScopeAsync(string serverName);

        /// <summary>
        /// Cleans the up server ath the end of the test.
        /// </summary>
        /// <param name="output">The output for the logs.</param>
        Task CleanUpAsync(ITestOutputHelper output);
    }
}