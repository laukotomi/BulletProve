using IntegTest.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IntegTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Initialize().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class HostExtensions
    {
        public static IHost Initialize(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

            seeder.Seed();

            return host;
        }
    }
}
