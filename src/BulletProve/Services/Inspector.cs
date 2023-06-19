﻿namespace BulletProve.Services
{
    /// <summary>
    /// The log filter.
    /// </summary>
    public sealed class Inspector<TInput> : IDisposable
        where TInput : class
    {
        private readonly List<AllowedAction<TInput>> _allowedActions = new();

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

        public bool IsAllowed(TInput item) => _allowedActions.Any(x => x.Action(item));

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
        public void Dispose()
        {
            Reset();
        }
    }

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