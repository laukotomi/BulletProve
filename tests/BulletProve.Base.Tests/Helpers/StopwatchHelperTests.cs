using BulletProve.Helpers;
using FluentAssertions;

namespace BulletProve.Tests.Helpers
{
    /// <summary>
    /// The stopwatch helper tests.
    /// </summary>
    public class StopwatchHelperTests
    {
        /// <summary>
        /// Tests the measure method.
        /// </summary>
        [Fact]
        public void TestMeasureAction()
        {
            var run = false;
            var elapsed = StopwatchHelper.Measure(() =>
            {
#pragma warning disable S2925 // "Thread.Sleep" should not be used in tests
                Thread.Sleep(10);
#pragma warning restore S2925 // "Thread.Sleep" should not be used in tests
                run = true;
            });

            run.Should().BeTrue();
            elapsed.Should().BeGreaterThan(5);
        }

        /// <summary>
        /// Tests the measure method.
        /// </summary>
        [Fact]
        public void TestMeasureFunc()
        {
            var result = StopwatchHelper.Measure(() =>
            {
#pragma warning disable S2925 // "Thread.Sleep" should not be used in tests
                Thread.Sleep(10);
#pragma warning restore S2925 // "Thread.Sleep" should not be used in tests
                return string.Empty;
            });

            result.ResultObject.Should().Be(string.Empty);
            result.ElapsedMilliseconds.Should().BeGreaterThan(5);
        }

        /// <summary>
        /// Tests the MeasureAsync method.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestMeasureAsync()
        {
            var run = false;
            var elapsed = await StopwatchHelper.MeasureAsync(async () =>
            {
                await Task.Delay(10);
                run = true;
            });

            run.Should().BeTrue();
            elapsed.Should().BeGreaterThan(5);
        }

        /// <summary>
        /// Tests the MeasureAsync with value.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestMeasureAsyncValue()
        {
            var result = await StopwatchHelper.MeasureAsync(async () =>
            {
                await Task.Delay(10);
                return string.Empty;
            });

            result.ResultObject.Should().Be(string.Empty);
            result.ElapsedMilliseconds.Should().BeGreaterThan(5);
        }
    }
}
