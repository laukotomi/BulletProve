using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        Task<LTestFacade> InitScopeAsync(string serverName);
        Task CleanUpAsync(ITestOutputHelper output);
    }
}