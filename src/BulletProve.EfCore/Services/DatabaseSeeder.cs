using BulletProve.Base.Hooks;
using BulletProve.Helpers;
using BulletProve.Logging;

namespace BulletProve.EfCore.Services
{
    /// <summary>
    /// The database seeder.
    /// </summary>
    public abstract class DatabaseSeeder : IBeforeTestHook
    {
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseSeeder"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected DatabaseSeeder(ITestLogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Seeds the database.
        /// </summary>
        protected abstract Task SeedAsync();

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(SeedAsync);
            _logger.LogInformation($"Seed done ({elapsedMs} ms)");
        }
    }
}
