using BulletProve.EfCore.Services;
using BulletProve.Logging;
using Example.Api.Services;

namespace Example.Api.IntegrationTests.Hooks
{
    /// <summary>
    /// The seed database hook.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeedDatabase"/> class.
    /// </remarks>
    /// <param name="seeder">The service provider.</param>
    /// <param name="testLogger">The test logger.</param>
    public class SeedDatabase(
        Seeder seeder,
        ITestLogger testLogger) : DatabaseSeeder(testLogger)
    {
        /// <inheritdoc/>
        protected override Task SeedAsync()
        {
            return seeder.SeedAsync();
        }
    }
}
