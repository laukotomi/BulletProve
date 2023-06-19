using BulletProve.EfCore.Services;
using BulletProve.Helpers;
using BulletProve.Hooks;
using BulletProve.Logging;
using Microsoft.EntityFrameworkCore;

namespace BulletProve.EfCore.Hooks
{
    /// <summary>
    /// Cleans database using EF features.
    /// </summary>
    public class CleanDatabaseHook<TDbContext> : IBeforeTestHook
        where TDbContext : DbContext
    {
        private readonly DatabaseCleanupService _databaseCleanupService = new();
        private readonly TDbContext _dbContext;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDatabaseHook{TDbContext}"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="testLogger">Logger.</param>
        public CleanDatabaseHook(
            TDbContext dbContext,
            ITestLogger testLogger)
        {
            _dbContext = dbContext;
            _testLogger = testLogger;
        }

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(() => _databaseCleanupService.CleanupAsync(_dbContext));

            _testLogger.LogInformation($"DB cleaned ({elapsedMs} ms)");
        }
    }
}
