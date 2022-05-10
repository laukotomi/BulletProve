using LTest.Interfaces;

namespace LTest
{
    /// <summary>
    /// A service that will be run before all tests. The services will be run in the same order as they were registered into DI.
    /// </summary>
    public interface IBeforeTestBehavior : IRunnable
    {
    }
}