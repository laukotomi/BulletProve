using BulletProve.Base.Configuration;

namespace BulletProve.Base.TestServer
{
    /// <summary>
    /// The test server factory.
    /// </summary>
    public class TestServerFactory : ITestServerFactory
    {
        /// <inheritdoc/>
        public ITestServer CreateTestServer<TStartup>(Action<ServerConfigurator>? configAction = null)
            where TStartup : class
        {
            return new TestServer<TStartup>(configAction);
        }
    }
}
