namespace LTest.Hooks
{
    /// <summary>
    /// A service that will be run before all tests. The services will be run in the same order as they were registered into DI.
    /// </summary>
    public interface IBeforeTestHook : IHook
    {
        /// <summary>
        /// Will be called at the beginning of each test in the same order as the service was registered.
        /// </summary>
        /// <returns>A Task.</returns>
        Task BeforeTestAsync();
    }
}
