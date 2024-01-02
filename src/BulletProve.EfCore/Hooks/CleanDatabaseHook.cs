using BulletProve.Base.Hooks;
using BulletProve.EfCore.Services;
using BulletProve.Helpers;
using BulletProve.Logging;
using Microsoft.EntityFrameworkCore;

namespace BulletProve.EfCore.Hooks
{
    /// <summary>
    /// Cleans database using EF features.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CleanDatabaseHook{TDbContext}"/> class.
    /// </remarks>
    /// <param name="dbContext">The db context.</param>
    /// <param name="testLogger">Logger.</param>
    public class CleanDatabaseHook<TDbContext>(
        TDbContext dbContext,
        ITestLogger testLogger,
        IDatabaseCleanupService databaseCleanupService) : IBeforeTestHook
        where TDbContext : DbContext
    {
        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(async () => await databaseCleanupService.CleanupAsync(dbContext));
            testLogger.LogInformation($"{dbContext.GetType().Name} cleaned ({elapsedMs} ms)");
        }
    }
}
