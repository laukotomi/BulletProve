using LTest.EFCore.Services;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using Microsoft.EntityFrameworkCore;

namespace LTest.EFCore.Behaviors
{
    /// <summary>
    /// Cleans database using EF features.
    /// </summary>
    public class CleanDatabaseHook<TDbContext> : IBeforeTestHook
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private readonly DatabaseCleanupService _databaseCleanupService;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDatabaseHook{TDbContext}"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="databaseCleanupService">Database cleanup service.</param>
        /// <param name="testLogger">Logger.</param>
        public CleanDatabaseHook(
            TDbContext dbContext,
            DatabaseCleanupService databaseCleanupService,
            ITestLogger testLogger)
        {
            _dbContext = dbContext;
            _databaseCleanupService = databaseCleanupService;
            _testLogger = testLogger;
        }

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(async () =>
            {
                await _databaseCleanupService.CleanupAsync(_dbContext);
            });

            _testLogger.LogInformation($"DB cleaned ({elapsedMs} ms)");
        }
    }
}
