using LTest.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LTest.Helpers
{
    /// <summary>
    /// Hook helper.
    /// </summary>
    public static class HookHelper
    {
        /// <summary>
        /// Runs the hooks.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="methodToRun">The method to run.</param>
        /// <returns>A Task.</returns>
        public static async Task RunHooksAsync<THook>(IServiceProvider services, Func<THook, Task> methodToRun)
        {
            var hooks = services.GetServices<THook>();
            var logger = services.GetRequiredService<ITestLogger>();
            using var scope = logger.Scope(typeof(THook).Name);

            if (hooks != null)
            {
                foreach (var hook in hooks)
                {
                    await methodToRun(hook);
                }
            }
        }
    }
}