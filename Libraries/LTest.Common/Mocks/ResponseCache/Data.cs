namespace LTest.Mocks.ResponseCache
{
    /// <summary>
    /// Data stored in cache.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Serialized request.
        /// </summary>
        public Request Request { get; set; }

        /// <summary>
        /// Response.
        /// </summary>
        public Response Response { get; set; }
    }
}