using LTest.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace LTest.Http.Services
{
    /// <summary>
    /// A service that can determine the HTTP method of a controller action.
    /// </summary>
    public class HttpMethodService
    {
        private readonly Dictionary<Type, HttpMethod> _httpAttributeMethodMap = new()
        {
            { typeof(HttpGetAttribute), HttpMethod.Get },
            { typeof(HttpPostAttribute), HttpMethod.Post },
            { typeof(HttpPutAttribute), HttpMethod.Put },
            { typeof(HttpPatchAttribute), HttpMethod.Patch },
            { typeof(HttpDeleteAttribute), HttpMethod.Delete },
            { typeof(HttpHeadAttribute), HttpMethod.Head },
            { typeof(HttpOptionsAttribute), HttpMethod.Options },
        };

        /// <summary>
        /// Returns the <see cref="HttpMethod"/> for a controller action.
        /// </summary>
        /// <param name="action">Controller action.</param>
        public HttpMethod GetHttpMethodForAction(MethodInfo action)
        {
            foreach (var attribute in action.CustomAttributes)
            {
                if (_httpAttributeMethodMap.TryGetValue(attribute.AttributeType, out var httpMethod))
                {
                    return httpMethod;
                }
            }

            throw new BulletProveException($"Could not determine {nameof(HttpMethod)} of action {action.Name}.");
        }
    }
}