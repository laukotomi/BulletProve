using LTest.EFCore.Services;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace LTest.EFCore.Behaviors
{
    /// <summary>
    /// Cleans database using EF features.
    /// </summary>
    public class CleanDatabaseBehavior<TDbContext> : IBeforeTestHook
        where TDbContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DatabaseCleanupService _databaseCleanupService;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanDatabaseBehavior{TDbContext}"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="databaseCleanupService">Database cleanup service.</param>
        /// <param name="testLogger">Logger.</param>
        public CleanDatabaseBehavior(
            IServiceProvider serviceProvider,
            DatabaseCleanupService databaseCleanupService,
            ITestLogger testLogger)
        {
            _serviceProvider = serviceProvider;
            _databaseCleanupService = databaseCleanupService;
            _testLogger = testLogger;
        }

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<TDbContext>();
                await _databaseCleanupService.CleanupAsync(dbContext);
            });

            _testLogger.LogInformation($"DB cleaned ({elapsedMs} ms)");
        }
    }
}
