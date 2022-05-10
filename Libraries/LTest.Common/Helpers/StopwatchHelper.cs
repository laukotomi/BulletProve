using System;
using System.Diagnostics;

namespace LTest.Helpers
{
    /// <summary>
    /// Stopwatch helper.
    /// </summary>
    public static class StopwatchHelper
    {
        /// <summary>
        /// Measures action execution time.
        /// </summary>
        /// <param name="action">Action</param>
        public static TimeSpan MeasureTimeSpan(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.Elapsed;
        }

        /// <summary>
        /// Measures action execution time.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Action.</param>
        /// <param name="elapsed">Elapsed.</param>
        /// <returns></returns>
        public static T MeasureTimeSpan<T>(Func<T> action, out TimeSpan elapsed)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = action();
            sw.Stop();
            elapsed = sw.Elapsed;
            return result;
        }

        /// <summary>
        /// Measures action execution time in milliseconds.
        /// </summary>
        /// <param name="action">Action.</param>
        public static long MeasureMilliseconds(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measures action execution time in milliseconds.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Action.</param>
        /// <param name="elapsedMs">Elapsed milliseconds.</param>
        public static T MeasureMilliseconds<T>(Func<T> action, out long elapsedMs)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = action();
            sw.Stop();
            elapsedMs = sw.ElapsedMilliseconds;
            return result;
        }
    }
}