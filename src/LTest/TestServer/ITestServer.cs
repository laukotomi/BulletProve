using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        Task<ServerScope> InitScopeAsync(string serverName);
        Task CleanUpAsync(ITestOutputHelper output);
    }
}