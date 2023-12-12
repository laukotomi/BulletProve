using BulletProve.Exceptions;
using BulletProve.ExtensionMethods;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// A service that can return request uri for a controller action.
    /// </summary>
    public class LinkGeneratorService : ILinkGeneratorService
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

        /// <inheritdoc/>
        public Uri GetRequestUri(string actionName, string controllerName, Dictionary<string, string>? values = null)
        {
            actionName = actionName.TrimEnd("Async");
            controllerName = controllerName.TrimEnd("Controller");
            var path = _linkGenerator.GetPathByAction(actionName, controllerName, values);

            if (path == null)
            {
                throw new BulletProveException($"Could not generate uri for {controllerName}/{actionName}. Did you fill all of the uri parts (values: {(values == null ? "null" : JsonSerializer.Serialize(values))})?");
            }

            return new Uri(path, UriKind.RelativeOrAbsolute);
        }
    }
}