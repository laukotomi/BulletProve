using System;

namespace LTest.Interfaces
{
    /// <summary>
    /// Test server configuration
    /// </summary>
    public interface ITestServer : IDisposable
    {
        /// <summary>
        /// Starts the server
        /// </summary>
        bool EnsureServerStarted();

        /// <summary>
        /// Access service provider.
        /// </summary>
        IServiceProvider Services { get; }
    }
}