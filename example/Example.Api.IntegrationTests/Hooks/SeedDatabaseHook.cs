using BulletProve.Helpers;
using BulletProve.Hooks;
using BulletProve.Logging;
using Example.Api.Services;
using System.Threading.Tasks;

namespace Example.Api.IntegrationTests.Hooks
{
    /// <summary>
    /// The seed database hook.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeedDatabaseHook"/> class.
    /// </remarks>
    /// <param name="seeder">The service provider.</param>
    /// <param name="testLogger">The test logger.</param>
    public class SeedDatabaseHook(
        Seeder seeder,
        ITestLogger testLogger) : IBeforeTestHook
    {
        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(seeder.SeedAsync);

            testLogger.LogInformation($"Seed done ({elapsedMs} ms)");
        }
    }
}
