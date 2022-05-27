using Example.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Example.Api
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Initialize().Run();
        }

        /// <summary>
        /// Creates the host builder.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <returns>An IHostBuilder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    /// <summary>
    /// The host extensions.
    /// </summary>
    public static class HostExtensions
    {
        /// <summary>
        /// Runs the initialization.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns>An IHost.</returns>
        public static IHost Initialize(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

            seeder.SeedAsync().GetAwaiter().GetResult();

            return host;
        }
    }
}
