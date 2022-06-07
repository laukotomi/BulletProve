using LTest.Http.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
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
        private readonly LTestFacade _facade;
        private readonly Dictionary<string, string> _uriValues = new();
        private string? _label;

        /// <summary>
        /// Gets the request.
        /// </summary>
        public HttpRequestMessage Request { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestService"/> class.
        /// </summary>
        /// <param name="controllerName">Controller name.</param>
        /// <param name="actionName">Action name.</param>
        /// <param name="method">Http method.</param>
        /// <param name="facade"></param>
        public HttpRequestService(
            string controllerName,
            string actionName,
            HttpMethod method,
            LTestFacade facade)
        {
            _controllerName = controllerName;
            _actionName = actionName;
            _facade = facade;

            Request = new();
            Request.Method = method;
            _facade.DisposableCollertor.Add(Request);
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
            var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            _facade.DisposableCollertor.Add(stringContent);
            Request.Content = stringContent;

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

        #region Start assering with status methods
        /// <summary>
        /// Starts asserting with success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<HttpResponseMessage> AssertSuccess()
        {
            return AssertSuccess<HttpResponseMessage>();
        }

        /// <summary>
        /// Starts asserting with success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<TResponse> AssertSuccess<TResponse>()
            where TResponse : class
        {
            return Assert<TResponse>()
                .EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Starts asserting with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<HttpResponseMessage> AssertStatus(HttpStatusCode statusCode)
        {
            return AssertStatus<HttpResponseMessage>(statusCode);
        }

        /// <summary>
        /// Starts asserting with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<TResponse> AssertStatus<TResponse>(HttpStatusCode statusCode)
            where TResponse : class
        {
            return Assert<TResponse>()
                .AssertStatusCode(statusCode);
        }

        /// <summary>
        /// Starts asserting problem details with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<LTestProblemDetails> AssertProblemWithStatus(HttpStatusCode statusCode)
        {
            return AssertStatus<LTestProblemDetails>(statusCode);
        }
        #endregion

        #region Execute with status assertion methods
        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public Task<HttpResponseMessage> ExecuteSuccessAsync()
        {
            return ExecuteSuccessAsync<HttpResponseMessage>();
        }

        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public Task<TResponse> ExecuteSuccessAsync<TResponse>()
            where TResponse : class
        {
            return Assert<TResponse>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();
        }

        /// <summary>
        /// Executes the request asserting specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public Task<HttpResponseMessage> ExecuteAssertingStatusAsync(HttpStatusCode statusCode)
        {
            return ExecuteAssertingStatusAsync<HttpResponseMessage>(statusCode);
        }

        /// <summary>
        /// Executes the request asserting specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public Task<TResponse> ExecuteAssertingStatusAsync<TResponse>(HttpStatusCode statusCode)
            where TResponse : class
        {
            return Assert<TResponse>()
                .AssertStatusCode(statusCode)
                .ExecuteAsync();
        }

        /// <summary>
        /// Starts asserting problem details with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public Task<LTestProblemDetails> ExecuteAssertingProblemAndStatusAsync(HttpStatusCode statusCode)
        {
            return ExecuteAssertingStatusAsync<LTestProblemDetails>(statusCode);
        }
        #endregion

        /// <summary>
        /// Create assert builder.
        /// </summary>
        public AssertBuilder<HttpResponseMessage> Assert()
        {
            return Assert<HttpResponseMessage>();
        }

        /// <summary>
        /// Create assert builder.
        /// </summary>
        public AssertBuilder<TResponse> Assert<TResponse>()
            where TResponse : class
        {
            if (string.IsNullOrEmpty(_label))
            {
                var labelGenerator = _facade.GetRequiredService<LabelGeneratorService>();
                _label = labelGenerator.GetLabel();
            }

            var linkGeneratorService = _facade.GetRequiredService<LinkGeneratorService>();
            var requestUri = linkGeneratorService.GetRequestUri(_actionName, _controllerName, _uriValues);
            Request.RequestUri = requestUri;

            return new AssertBuilder<TResponse>(Request, _facade, _label);
        }
    }
}