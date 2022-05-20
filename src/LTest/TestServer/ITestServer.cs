namespace LTest
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        /// <summary>
        /// Access service provider.
        /// </summary>
        IServiceProvider Services { get; }
    }
}