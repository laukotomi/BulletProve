using LTest.Interfaces;

namespace LTest
{
    /// <summary>
    /// A service that will be run after server started. Because server instances are cached, it will be called only when a new test server is started.
    /// </summary>
    public interface IAfterServerStartedBehavior : IRunnable
    {
    }
}