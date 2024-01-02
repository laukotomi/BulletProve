namespace BulletProve.Base.Hooks
{
    /// <summary>
    /// The hook runner.
    /// </summary>
    public interface IHookRunner
    {
        /// <summary>
        /// Runs the hooks.
        /// </summary>
        /// <param name="methodToRun">The method to run.</param>
        Task RunHooksAsync<THook>(Func<THook, Task> methodToRun) where THook : IHook;
    }
}