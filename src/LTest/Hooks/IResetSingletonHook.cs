namespace LTest.Hooks
{
    /// <summary>
    /// A helper hook to reset singletons when the test starts. It won't run when the test server was just started.
    /// </summary>
    public interface IResetSingletonHook
    {
        /// <summary>
        /// Runs before every test in case the server was already running to clean singletons (usually mocks).
        /// </summary>
        /// <returns>A Task.</returns>
        Task ResetAsync();
    }
}
