using BulletProve.Hooks;

namespace BulletProve.Services
{
    /// <summary>
    /// The inspector.
    /// </summary>
    public sealed class Inspector<TInput> : IInspector<TInput>, ICleanUpHook
        where TInput : class
    {
        private readonly List<AllowedAction<TInput>> _allowedActions = new();

        /// <summary>
        /// Adds a new filter.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public Inspector<TInput> AddDefaultAllowedAction(Func<TInput, bool> action, string label)
        {
            _allowedActions.Add(new AllowedAction<TInput>(label, action, true));

            return this;
        }

        /// <summary>
        /// Adds a new filter.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public Inspector<TInput> AddAllowedAction(Func<TInput, bool> action, string? label = null)
        {
            _allowedActions.Add(new AllowedAction<TInput>(label, action, false));

            return this;
        }

        /// <summary>
        /// Clears the filters.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public Inspector<TInput> Clear()
        {
            _allowedActions.Clear();

            return this;
        }

        /// <summary>
        /// Resets the default filters.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public Inspector<TInput> Reset()
        {
            var toRemove = _allowedActions
                .Where(x => !x.IsDefault)
                .ToList();

            foreach (var item in toRemove)
            {
                _allowedActions.Remove(item);
            }

            return this;
        }

        /// <inheritdoc/>
        public bool IsAllowed(TInput item) => _allowedActions.Exists(x => x.Action(item));

        /// <summary>
        /// Removes a filter by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A LogFilter.</returns>
        public Inspector<TInput> Remove(string name)
        {
            var toRemove = _allowedActions
                .Where(x => x.ActionName?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();

            foreach (var item in toRemove)
            {
                _allowedActions.Remove(item);
            }

            return this;
        }

        /// <inheritdoc />
        public Task CleanUpAsync()
        {
            Reset();
            return Task.CompletedTask;
        }
    }
}
