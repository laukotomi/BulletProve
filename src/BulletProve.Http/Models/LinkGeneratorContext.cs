namespace BulletProve.Http.Models
{
    /// <summary>
    /// The link generator context.
    /// </summary>
    public class LinkGeneratorContext
    {
        /// <summary>
        /// Gets the action name.
        /// </summary>
        public string ActionName { get; }

        /// <summary>
        /// Gets the controller name.
        /// </summary>
        public string ControllerName { get; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        public HttpMethod Method { get; }

        /// <summary>
        /// Gets the uri values.
        /// </summary>
        public Dictionary<string, string> UriValues { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkGeneratorContext"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="controllerName">The controller name.</param>
        /// <param name="actionName">The action name.</param>
        public LinkGeneratorContext(HttpMethod method, string controllerName, string actionName)
        {
            Method = method;
            ControllerName = controllerName;
            ActionName = actionName;
        }
    }
}
