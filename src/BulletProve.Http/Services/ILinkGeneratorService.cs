namespace BulletProve.Http.Services
{
    /// <summary>
    /// The link generator service.
    /// </summary>
    public interface ILinkGeneratorService
    {
        /// <summary>
        /// Generates the request uri for a controller action.
        /// </summary>
        /// <param name="actionName">The name of the controller action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="values">Uri values if there are any.</param>
        public Uri GetRequestUri(string actionName, string controllerName, Dictionary<string, string>? values = null);
    }
}