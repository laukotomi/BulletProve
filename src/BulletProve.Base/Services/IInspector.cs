namespace BulletProve.Services
{
    /// <summary>
    /// The inspector.
    /// </summary>
    public interface IInspector<in TInput>
    {
        /// <summary>
        /// Is item allowed.
        /// </summary>
        /// <param name="item">The item.</param>
        bool IsAllowed(TInput item);
    }
}
