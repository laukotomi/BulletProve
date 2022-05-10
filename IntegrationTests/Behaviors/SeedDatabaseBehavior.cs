using IntegTest.Services;
using LTest;
using LTest.Helpers;
using LTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace IntegrationTests.Behaviors
{
    public class SeedDatabaseBehavior : IBeforeTestBehavior
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITestLogger _testLogger;

        public SeedDatabaseBehavior(
            IServiceProvider serviceProvider,
            ITestLogger testLogger)
        {
            _serviceProvider = serviceProvider;
            _testLogger = testLogger;
        }

        public Task RunAsync()
        {
            var elapsedMs = StopwatchHelper.MeasureMilliseconds(() =>
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var seeder = services.GetRequiredService<Seeder>();
                    seeder.Seed();
                }
            });

            _testLogger.Info($"Seed done ({elapsedMs} ms)");
            return Task.CompletedTask;
        }
    }
}
