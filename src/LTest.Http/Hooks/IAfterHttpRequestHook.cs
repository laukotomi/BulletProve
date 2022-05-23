namespace LTest.Http.Interfaces
{
    /// <summary>
    /// A service that will be run after server Http request was sent.
    /// </summary>
    public interface IAfterHttpRequestHook
    {
        /// <summary>
        /// Will be run after server Http request was sent.
        /// </summary>
        /// <param name="_label">The _label.</param>
        /// <param name="response">The response.</param>
        /// <returns>A Task.</returns>
        Task AfterHttpRequestAsync(string _label, HttpResponseMessage response);
    }
}