using System.Threading.Tasks;

namespace LTest.Interfaces
{
    /// <summary>
    /// Interface for behaviors.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Behavior logic.
        /// </summary>
        Task RunAsync();
    }
}