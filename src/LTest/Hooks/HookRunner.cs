using LTest.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace LTest.Hooks
{
    /// <summary>
    /// Hook helper.
    /// </summary>
    public class HookRunner
    {
        private readonly IServiceProvider _services;
        private readonly ITestLogger _logger;

        public HookRunner(IServiceProvider services, ITestLogger logger)
        {
            _services = services;
            _logger = logger;
        }

        /// <summary>
        /// Runs the hooks.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="methodToRun">The method to run.</param>
        /// <returns>A Task.</returns>
        public async Task RunHooksAsync<THook>(Func<THook, Task> methodToRun)
            where THook : IHook
        {
            using var scope = _logger.Scope(typeof(THook).Name);
            var hooks = _services.GetServices<THook>();

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