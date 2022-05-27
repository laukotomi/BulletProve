namespace LTest.Logging
{
    /// <summary>
    /// The log filter.
    /// </summary>
    public sealed class LogFilter<TInput> : IDisposable
        where TInput : class
    {
        private readonly IReadOnlyCollection<LogEventFilter<TInput>> _defaultExpectedEvents;
        private readonly List<LogEventFilter<TInput>> _expectedEvents;

        /// <summary>
        /// Gets the filters.
        /// </summary>
        public IReadOnlyCollection<LogEventFilter<TInput>> Filters => _expectedEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFilter{TInput}"/> class.
        /// </summary>
        /// <param name="defaultExpectedEvents">The default expected events.</param>
        public LogFilter(IEnumerable<LogEventFilter<TInput>>? defaultExpectedEvents = null)
        {
            if (defaultExpectedEvents == null)
            {
                _defaultExpectedEvents = new List<LogEventFilter<TInput>>();
                _expectedEvents = new List<LogEventFilter<TInput>>();
            }
            else
            {
                _defaultExpectedEvents = new List<LogEventFilter<TInput>>(defaultExpectedEvents);
                _expectedEvents = new List<LogEventFilter<TInput>>(defaultExpectedEvents);
            }
        }

        /// <summary>
        /// Clears the filters.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public LogFilter<TInput> Clear()
        {
            _expectedEvents.Clear();
            return this;
        }

        /// <summary>
        /// Resets the default filters.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public LogFilter<TInput> Reset()
        {
            Clear();
            _expectedEvents.AddRange(_defaultExpectedEvents);

            return this;
        }

        /// <summary>
        /// Adds a new filter.
        /// </summary>
        /// <returns>A LogFilter.</returns>
        public LogFilter<TInput> Add(Func<TInput, bool> action, string? name = null)
        {
            _expectedEvents.Add(new LogEventFilter<TInput>(name, action));
            return this;
        }

        /// <summary>
        /// Adds a new filter.
        /// </summary>
        /// <param name="expectedEvent">The expected event.</param>
        /// <returns>A LogFilter.</returns>
        public LogFilter<TInput> Add(LogEventFilter<TInput> expectedEvent)
        {
            _expectedEvents.Add(expectedEvent);
            return this;
        }

        /// <summary>
        /// Removes a filter by name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A LogFilter.</returns>
        public LogFilter<TInput> Remove(string name)
        {
            var toRemove = _expectedEvents
                .Where(x => x.FilterName?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false)
                .ToList();

            foreach (var expectedEvent in toRemove)
            {
                _expectedEvents.Remove(expectedEvent);
            }

            return this;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            Reset();
        }
    }

    /// <summary>
    /// The log event filter.
    /// </summary>
    public class LogEventFilter<TInput>
        where TInput : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEventFilter{TInput}"/> class.
        /// </summary>
        /// <param name="filterName">The filter name.</param>
        /// <param name="action">The action.</param>
        public LogEventFilter(string? filterName, Func<TInput, bool> action)
        {
            FilterName = filterName;
            Action = action;
        }

        /// <summary>
        /// Gets the filter name.
        /// </summary>
        public string? FilterName { get; }

        /// <summary>
        /// Gets the filter action.
        /// </summary>
        public Func<TInput, bool> Action { get; }
    }
}
