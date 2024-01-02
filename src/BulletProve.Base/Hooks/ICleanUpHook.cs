namespace BulletProve.Base.Hooks
{
    /// <summary>
    /// A service that will be run after all tests. The services will be run in the same order as they were registered into DI.
    /// </summary>
    public interface ICleanUpHook : IHook
    {
        /// <summary>
        /// Clans up internals of the class at the end of the test.
        /// </summary>
        Task CleanUpAsync();
    }
}
