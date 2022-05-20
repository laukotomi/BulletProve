namespace LTest.Logging
{
    /// <summary>
    /// Test logger scope.
    /// </summary>
    public sealed class TestLoggerScope : IDisposable
    {
        private readonly ITestLogger _logger;
        private readonly Action<TestLoggerScope> _finishAction;
        private bool _finalized;

        /// <summary>
        /// Scope level.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerScope"/> class.
        /// </summary>
        /// <param name="logger">Test logger.</param>
        /// <param name="level">Scope level</param>
        /// <param name="finishAction">Finish action.</param>
        public TestLoggerScope(ITestLogger logger, int level, Action<TestLoggerScope> finishAction)
        {
            _logger = logger;
            Level = level;
            _finishAction = finishAction;
        }

        /// <summary>
        /// Finish action (same as Dispose, but enables logging).
        /// </summary>
        public void Finish(Action<ITestLogger>? logAction = null)
        {
            Dispose();
            logAction?.Invoke(_logger);

            if (Level <= 2)
            {
                _logger.LogEmptyLine();
            }
        }

        /// <summary>
        /// Dispose action.
        /// </summary>
        public void Dispose()
        {
            if (!_finalized)
            {
                _finishAction(this);
                _finalized = true;
            }
        }
    }
}