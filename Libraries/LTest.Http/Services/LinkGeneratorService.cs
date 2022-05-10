using LTest.ExtensionMethods;
using Microsoft.AspNetCore.Routing;
using System;

namespace LTest.Http.Services
{
    /// <summary>
    /// A service that can return request uri for a controller action.
    /// </summary>
    public class LinkGeneratorService
    {
        private readonly LinkGenerator _linkGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkGeneratorService"/> class.
        /// </summary>
        /// <param name="linkGenerator"><see cref="LinkGenerator"/> service.</param>
        public LinkGeneratorService(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        /// <summary>
        /// Generates the request uri for a controller action.
        /// </summary>
        /// <param name="actionName">The name of the controller action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="values">Uri values if there are any.</param>
        public Uri GetRequestUri(string actionName, string controllerName, object values = null)
        {
            actionName = actionName.TrimEnd("Async");
            controllerName = controllerName.TrimEnd("Controller");
            var path = _linkGenerator.GetPathByAction(actionName, controllerName, values);

            if (path == null)
            {
                throw new InvalidOperationException($"Could not generate uri for {controllerName}/{actionName}. Did you fill all of the uri parts (values: {(values == null ? "null" : values)})?");
            }

            return new Uri(path, UriKind.RelativeOrAbsolute);
        }
    }
}