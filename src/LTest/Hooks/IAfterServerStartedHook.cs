namespace LTest.Hooks
{
    /// <summary>
    /// A service that will be run after server started. Because server instances are cached, it will be called only when a new test server is created.
    /// </summary>
    public interface IAfterServerStartedHook
    {
        /// <summary>
        /// Will be called after the test server started in the same order as the service was registered.
        /// </summary>
        /// <returns>A Task.</returns>
        Task AfterServerStartedAsync();
    }
}
