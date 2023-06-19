using System.Diagnostics;

namespace BulletProve.Helpers
{
    /// <summary>
    /// Stopwatch helper.
    /// </summary>
    public static class StopwatchHelper
    {
        /// <summary>
        /// Measures the execution of the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public static long Measure(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measures the execution of the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public static StopwachHelperResult<T> Measure<T>(Func<T> action)
            where T : class
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = action();
            sw.Stop();

            return new StopwachHelperResult<T>
            {
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
                ResultObject = result
            };
        }

        /// <summary>
        /// Measures the execution of the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The elapsed milliseconds.</returns>
        public static async Task<long> MeasureAsync(Func<Task> action)
        {
            var sw = new Stopwatch();
            sw.Start();
            await action();
            sw.Stop();

            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measures the execution of the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public static async Task<StopwachHelperResult<T>> MeasureAsync<T>(Func<Task<T>> action)
            where T : class
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = await action();
            sw.Stop();

            return new StopwachHelperResult<T>
            {
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
                ResultObject = result
            };
        }
    }

    /// <summary>
    /// The stopwach helper result.
    /// </summary>
    public class StopwachHelperResult<T>
        where T : class
    {
        /// <summary>
        /// Gets the result object.
        /// </summary>
        public T ResultObject { get; init; } = null!;

        /// <summary>
        /// Gets the elapsed milliseconds.
        /// </summary>
        public long ElapsedMilliseconds { get; init; }
    }
}