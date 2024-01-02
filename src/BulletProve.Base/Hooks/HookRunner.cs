using BulletProve.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace BulletProve.Base.Hooks
{
    /// <summary>
    /// Hook helper.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="HookRunner"/> class.
    /// </remarks>
    /// <param name="services">The services.</param>
    /// <param name="logger">The logger.</param>
    public class HookRunner(IServiceProvider services, ITestLogger logger) : IHookRunner
    {
        /// <inheritdoc/>
        public async Task RunHooksAsync<THook>(Func<THook, Task> methodToRun)
            where THook : IHook
        {
            var hookName = typeof(THook).Name;
            var hooks = services.GetServices<THook>();

            if (hooks != null)
            {
                foreach (var hook in hooks.Distinct())
                {
                    using var scope = logger.Scope($"{hook.GetType().Name} ({hookName})");
                    await methodToRun(hook);
                }
            }
        }
    }
}