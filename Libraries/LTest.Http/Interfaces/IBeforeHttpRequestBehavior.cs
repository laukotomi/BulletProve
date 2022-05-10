using LTest.Interfaces;

namespace LTest.Http.Interfaces
{
    /// <summary>
    /// A service that will be run before server Http request.
    /// </summary>
    public interface IBeforeHttpRequestBehavior : IRunnable
    {
    }
}