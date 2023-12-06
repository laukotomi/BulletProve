namespace BulletProve.Services
{
    /// <summary>
    /// The log event filter.
    /// </summary>
    public class AllowedAction<TInput>
        where TInput : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventFilter{TInput}"/> class.
        /// </summary>
        /// <param name="filterName">The filter name.</param>
        /// <param name="action">The action.</param>
        public AllowedAction(string? actionName, Func<TInput, bool> action, bool isDefault)
        {
            ActionName = actionName;
            Action = action;
            IsDefault = isDefault;
        }

        /// <summary>
        /// Gets a value indicating whether is default action.
        /// </summary>
        public bool IsDefault { get; }

        /// <summary>
        /// Gets the filter name.
        /// </summary>
        public string? ActionName { get; }

        /// <summary>
        /// Gets the filter action.
        /// </summary>
        public Func<TInput, bool> Action { get; }
    }
}
