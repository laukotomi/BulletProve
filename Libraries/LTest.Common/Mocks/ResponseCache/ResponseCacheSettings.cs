namespace LTest.Mocks.ResponseCache
{
    /// <summary>
    /// Response cache settings.
    /// </summary>
    public class ResponseCacheSettings
    {
        /// <summary>
        /// Response cache directory. Use {outputdir} to specify the output (bin) directory of the integration test project.
        /// </summary>
        public string ResponseCacheDir { get; set; }

        /// <summary>
        /// Whether to enable adding request to the cache.
        /// </summary>
        public bool EnableAddingRequest { get; set; }

        /// <summary>
        /// Whether to enable generating response for the request.
        /// </summary>
        public bool EnableGeneratingResponse { get; set; }
    }
}