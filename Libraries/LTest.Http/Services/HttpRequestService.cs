using LTest.Interfaces;
using LTest.LogSniffer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;

namespace LTest.Http.Services
{
    /// <summary>
    /// Helper class to build and send the HTTP request.
    /// </summary>
    public class HttpRequestService
    {
        private readonly ILogSnifferService _logSnifferService;
        private readonly string _controllerName;
        private readonly string _actionName;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpRequestMessage _request = new HttpRequestMessage();
        private readonly Dictionary<string, string> _uriValues = new();
        private readonly LinkGeneratorService _linkGeneratorService;
        private bool _autoRestoreUnexpectedLogSnifferEventAction;
        private string _label;

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
            _request.Method = method;
            _controllerName = controllerName;
            _actionName = actionName;
            _serviceProvider = serviceProvider;
            _logSnifferService = serviceProvider.GetRequiredService<ILogSnifferService>();
            _linkGeneratorService = serviceProvider.GetRequiredService<LinkGeneratorService>();
        }

        /// <summary>
        /// Helper method to set HTTP headers in a chainable way.
        /// </summary>
        /// <param name="httpHeadersAction">Lambda to set the HTTP headers.</param>
        public HttpRequestService SetHeaders(Action<HttpRequestHeaders> httpHeadersAction)
        {
            httpHeadersAction(_request.Headers);
            return this;
        }

        /// <summary>
        /// Helper method to set json HTTP content.
        /// </summary>
        /// <param name="content">The json object.</param>
        public HttpRequestService SetJsonContent(object content)
        {
            var json = JsonConvert.SerializeObject(content);
            _request.Content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

            return this;
        }

        /// <summary>
        /// Helper method to set the HTTP content in a chainable way.
        /// </summary>
        /// <param name="content">HTTP content object.</param>
        public HttpRequestService SetContent(HttpContent content)
        {
            _request.Content = content;
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
            configureAction(_request);
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
        /// Overrides the global is expected logs action. Pass null to restore changes.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="autoRestoreAfterSend">Whether to automatically restore is unexpected log sniffer event action.</param>
        public HttpRequestService OverrideIsExpectedLogSnifferLogAction(Func<LogSnifferEvent, bool> action, bool autoRestoreAfterSend = true)
        {
            _logSnifferService.OverrideIsExpectedLogEventAction(action);
            _autoRestoreUnexpectedLogSnifferEventAction = autoRestoreAfterSend;

            return this;
        }

        /// <summary>
        /// Starts assertion builder phase.
        /// </summary>
        public AssertBuilder<EmptyResponse> Assert()
        {
            return Assert<EmptyResponse>();
        }

        /// <summary>
        /// Starts assertion builder phase.
        /// </summary>
        public AssertBuilder<TResponse> Assert<TResponse>()
            where TResponse : class
        {
            var requestUri = _linkGeneratorService.GetRequestUri(_actionName, _controllerName, _uriValues);
            _request.RequestUri = requestUri;

            return new AssertBuilder<TResponse>(_request, _serviceProvider, _autoRestoreUnexpectedLogSnifferEventAction, _label);
        }

        /// <summary>
        /// Generates curl from the request.
        /// </summary>
        public string ToCurl()
        {
            return _request.ToCurl();
        }
    }
}