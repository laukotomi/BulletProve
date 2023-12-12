using BulletProve.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Hooks
{
    /// <summary>
    /// Hook helper.
    /// </summary>
    public class HookRunner : IHookRunner
    {
        private readonly IServiceProvider _services;
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HookRunner"/> class.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="logger">The logger.</param>
        public HookRunner(IServiceProvider services, ITestLogger logger)
        {
            _services = services;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task RunHooksAsync<THook>(Func<THook, Task> methodToRun)
            where THook : IHook
        {
            using var scope = _logger.Scope(typeof(THook).Name);
            var hooks = _services.GetServices<THook>();

            if (hooks != null)
            {
                foreach (var hook in hooks.Distinct())
                {
                    await methodToRun(hook);
                }
            }
        }
    }
}