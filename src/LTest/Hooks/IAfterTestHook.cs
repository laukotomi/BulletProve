namespace LTest.Hooks
{
    /// <summary>
    /// A service that will be run after all tests. The services will be run in the same order as they were registered into DI.
    /// </summary>
    public interface IAfterTestHook : IHook
    {
        /// <summary>
        /// Will be called at the end of each test in the same order as the service was registered.
        /// </summary>
        /// <returns>A Task.</returns>
        Task AfterTestAsync();
    }
}
