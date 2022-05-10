using LTest.Interfaces;

namespace LTest.Http.Interfaces
{
    /// <summary>
    /// A service that will be run after server Http request was send.
    /// </summary>
    public interface IAfterHttpRequestBehavior : IRunnable
    {
    }
}