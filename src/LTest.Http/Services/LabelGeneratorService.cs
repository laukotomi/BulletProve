using LTest.Hooks;
using System.Threading;
using System.Threading.Tasks;

namespace LTest.Http.Services
{
    /// <summary>
    /// The label generator service.
    /// </summary>
    internal class LabelGeneratorService : IResetSingletonHook
    {
        private int _counter = 0;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetLabel()
        {
            return $"#{Interlocked.Increment(ref _counter)}";
        }

        /// <summary>
        /// Resets the counter.
        /// </summary>
        /// <returns></returns>
        public Task ResetAsync()
        {
            _counter = 0;
            return Task.CompletedTask;
        }
    }
}
