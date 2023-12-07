using BulletProve.TestServer;

namespace BulletProve
{
    /// <summary>
    /// The test server factory.
    /// </summary>
    public interface ITestServerFactory
    {
        /// <summary>
        /// Creates the test server.
        /// </summary>
        /// <param name="configAction">The config action.</param>
        /// <returns>An ITestServer.</returns>
        ITestServer CreateTestServer<TStartup>(Action<ServerConfigurator>? configAction = null)
            where TStartup : class;
    }
}
