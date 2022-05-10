using LTest.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LTest.Behaviors
{
    /// <summary>
    /// Clean before test behavior.
    /// </summary>
    public class CleanSingletonsBeforeTestBehavior : IBeforeTestBehavior
    {
        private readonly IEnumerable<ICleanSingletonBeforeTest> _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="CleanSingletonsBeforeTestBehavior"/> class.
        /// </summary>
        /// <param name="services">Services.</param>
        public CleanSingletonsBeforeTestBehavior(IEnumerable<ICleanSingletonBeforeTest> services)
        {
            _services = services;
        }

        /// <summary>
        /// Behavior logic.
        /// </summary>
        public Task RunAsync()
        {
            foreach (var service in _services)
            {
                service.Clear();
            }

            return Task.CompletedTask;
        }
    }
}