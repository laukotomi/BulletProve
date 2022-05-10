using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;

namespace LTest.Http.Services
{
    /// <summary>
    /// A service that can determine the HTTP method of a controller action.
    /// </summary>
    public class HttpMethodService
    {
        private readonly Dictionary<Type, HttpMethod> _httpAttributeMethodMap = new Dictionary<Type, HttpMethod>
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

            throw new InvalidOperationException($"Could not determine {nameof(HttpMethod)}.");
        }
    }
}