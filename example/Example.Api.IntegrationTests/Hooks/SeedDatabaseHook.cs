using Example.Api.Services;
using LTest.Helpers;
using LTest.Hooks;
using LTest.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Example.Api.IntegrationTests.Hooks
{
    /// <summary>
    /// The seed database hook.
    /// </summary>
    public class SeedDatabaseHook : IBeforeTestHook
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITestLogger _testLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedDatabaseHook"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="testLogger">The test logger.</param>
        public SeedDatabaseHook(
            IServiceProvider serviceProvider,
            ITestLogger testLogger)
        {
            _serviceProvider = serviceProvider;
            _testLogger = testLogger;
        }

        /// <inheritdoc/>
        public async Task BeforeTestAsync()
        {
            var elapsedMs = await StopwatchHelper.MeasureAsync(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var services = scope.ServiceProvider;
                var seeder = services.GetRequiredService<Seeder>();
                await seeder.SeedAsync();
            });

            _testLogger.LogInformation($"Seed done ({elapsedMs} ms)");
        }
    }
}
