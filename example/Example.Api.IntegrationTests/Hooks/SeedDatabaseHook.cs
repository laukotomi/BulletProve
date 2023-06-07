using Example.Api.Services;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using System.Threading.Tasks;

namespace Example.Api.IntegrationTests.Hooks
{
    /// <summary>
    /// The seed database hook.
    /// </summary>
    public class SeedDatabaseHook : IBeforeTestHook
    {
        private readonly Seeder _seeder;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedDatabaseHook"/> class.
        /// </summary>
        /// <param name="seeder">The service provider.</param>
        /// <param name="testLogger">The test logger.</param>
        public SeedDatabaseHook(
            Seeder seeder,
            ITestLogger testLogger)
        {
            _seeder = seeder;
            _testLogger = testLogger;
        }

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(_seeder.SeedAsync);

            _testLogger.LogInformation($"Seed done ({elapsedMs} ms)");
        }
    }
}
