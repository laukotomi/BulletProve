using LTest.Interfaces;
using System;

namespace LTest.Models
{
    /// <summary>
    /// Server details.
    /// </summary>
    public sealed class ServerDetails : IDisposable
    {
        private readonly ITestServer _testServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerDetails"/> class.
        /// </summary>
        /// <param name="testServer">Test server</param>
        public ServerDetails(ITestServer testServer)
        {
            _testServer = testServer;
            Services = new IntegrationTestServiceProvider(testServer.Services);
        }

        /// <summary>
        /// Access services using this property.
        /// </summary>
        public IntegrationTestServiceProvider Services { get; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            _testServer.Dispose();
        }
    }
}