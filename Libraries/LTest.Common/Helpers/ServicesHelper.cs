using LTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LTest.Helpers
{
    /// <summary>
    /// Services helper.
    /// </summary>
    public static class ServicesHelper
    {
        /// <summary>
        /// Run services of <see cref="IRunnable"/>.
        /// </summary>
        /// <typeparam name="TRunnable"><see cref="IRunnable"/> type.</typeparam>
        /// <param name="services">Service provider.</param>
        public static void RunServices<TRunnable>(IServiceProvider services)
            where TRunnable : IRunnable
        {
            var runnables = services.GetServices<TRunnable>();
            if (runnables != null)
            {
                foreach (var runnable in runnables)
                {
                    runnable.RunAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}