using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace LTest.Http.Services
{
    /// <summary>
    /// Helper class to build and send the HTTP request.
    /// </summary>
    public class HttpRequestService
    {
        private readonly string _controllerName;
        private readonly string _actionName;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, string> _uriValues = new();
        private readonly LinkGeneratorService _linkGeneratorService;
        private string _label;

        /// <summary>
        /// Gets the request.
        /// </summary>
        public HttpRequestMessage Request { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestService"/> class.
        /// </summary>
        /// <param name="controllerName">Controller name.</param>
        /// <param name="actionName">Action name.</param>
        /// <param name="method">Http method.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public HttpRequestService(
            string controllerName,
            string actionName,
            HttpMethod method,
            IServiceProvider serviceProvider)
        {
            Request.Method = method;
            _controllerName = controllerName;
            _actionName = actionName;
            _serviceProvider = serviceProvider;
            _linkGeneratorService = serviceProvider.GetRequiredService<LinkGeneratorService>();

            var labelGenerator = serviceProvider.GetRequiredService<LabelGeneratorService>();
            _label = labelGenerator.GetLabel();
        }

        /// <summary>
        /// Helper method to set HTTP headers in a chainable way.
        /// </summary>
        /// <param name="httpHeadersAction">Lambda to set the HTTP headers.</param>
        public HttpRequestService SetHeaders(Action<HttpRequestHeaders> httpHeadersAction)
        {
            httpHeadersAction(Request.Headers);
            return this;
        }

        /// <summary>
        /// Helper method to set json HTTP content.
        /// </summary>
        /// <param name="content">The json object.</param>
        public HttpRequestService SetJsonContent(object content)
        {
            var json = JsonSerializer.Serialize(content);
            Request.Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            return this;
        }

        /// <summary>
        /// Helper method to set the HTTP content in a chainable way.
        /// </summary>
        /// <param name="content">HTTP content object.</param>
        public HttpRequestService SetContent(HttpContent content)
        {
            Request.Content = content;
            return this;
        }

        /// <summary>
        /// Sets uri values.
        /// </summary>
        /// <param name="action">Action to set uri values.</param>
        public HttpRequestService SetUriValues(Action<Dictionary<string, string>> action)
        {
            action?.Invoke(_uriValues);

            return this;
        }

        /// <summary>
        /// Helper method to configure anything for the HTTP request in a chainable way.
        /// </summary>
        /// <param name="configureAction">The action to configure the HTTP request.</param>
        public HttpRequestService ConfigureHttpRequest(Action<HttpRequestMessage> configureAction)
        {
            configureAction(Request);
            return this;
        }

        /// <summary>
        /// Adds label to the request which will be added to logs.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public HttpRequestService SetLabel(string label)
        {
            _label = label;
            return this;
        }

        /// <summary>
        /// Starts assertion builder phase.
        /// </summary>
        public AssertBuilder<HttpResponseMessage> Assert()
        {
            return Assert<HttpResponseMessage>();
        }

        /// <summary>
        /// Starts assertion builder phase.
        /// </summary>
        public AssertBuilder<TResponse> Assert<TResponse>()
            where TResponse : class
        {
            var requestUri = _linkGeneratorService.GetRequestUri(_actionName, _controllerName, _uriValues);
            Request.RequestUri = requestUri;

            return new AssertBuilder<TResponse>(Request, _serviceProvider, _label);
        }
    }
}