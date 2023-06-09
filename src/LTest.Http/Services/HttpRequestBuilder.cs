using LTest.Http.Models;
using LTest.Logging;
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
    public class HttpRequestBuilder
    {
        private readonly LinkGeneratorContext _linkGeneratorContext;
        private readonly LTestFacade _facade;
        private readonly LinkGeneratorService _linkGeneratorService;

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public HttpRequestContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class.
        /// </summary>
        /// <param name="controllerName">Controller name.</param>
        /// <param name="actionName">Action name.</param>
        /// <param name="method">Http method.</param>
        /// <param name="facade"></param>
        public HttpRequestBuilder(LinkGeneratorContext linkGeneratorContext, LTestFacade facade)
        {
            _linkGeneratorContext = linkGeneratorContext;
            _facade = facade;

            Context = new HttpRequestContext(facade.GetRequiredService<LabelGeneratorService>().GetLabel());
            Context.Request.Method = _linkGeneratorContext.Method;
            _facade.DisposableCollertor.Add(Context);
            _linkGeneratorService = _facade.GetRequiredService<LinkGeneratorService>();
        }

        /// <summary>
        /// Helper method to set HTTP headers in a chainable way.
        /// </summary>
        /// <param name="httpHeadersAction">Lambda to set the HTTP headers.</param>
        public HttpRequestBuilder SetHeaders(Action<HttpRequestHeaders> httpHeadersAction)
        {
            httpHeadersAction(Context.Request.Headers);
            return this;
        }

        /// <summary>
        /// Helper method to set json HTTP content.
        /// </summary>
        /// <param name="content">The json object.</param>
        public HttpRequestBuilder SetJsonContent(object content)
        {
            var json = JsonSerializer.Serialize(content);
            var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            _facade.DisposableCollertor.Add(stringContent);
            Context.Request.Content = stringContent;

            return this;
        }

        /// <summary>
        /// Helper method to set the HTTP content in a chainable way.
        /// </summary>
        /// <param name="content">HTTP content object.</param>
        public HttpRequestBuilder SetContent(HttpContent content)
        {
            Context.Request.Content = content;
            return this;
        }

        /// <summary>
        /// Sets uri values.
        /// </summary>
        /// <param name="action">Action to set uri values.</param>
        public HttpRequestBuilder SetQueryParameters(Action<Dictionary<string, string>> action)
        {
            action?.Invoke(_linkGeneratorContext.UriValues);

            return this;
        }

        /// <summary>
        /// Helper method to configure anything for the HTTP request in a chainable way.
        /// </summary>
        /// <param name="configureAction">The action to configure the HTTP request.</param>
        public HttpRequestBuilder ConfigureHttpRequest(Action<HttpRequestMessage> configureAction)
        {
            configureAction(Context.Request);
            return this;
        }

        /// <summary>
        /// Adds label to the request which will be added to logs.
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public HttpRequestBuilder SetLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            Context.Label = label;
            return this;
        }

        public HttpRequestBuilder ExportToCurl(out string curl)
        {
            SetRequestUri();
            curl = Context.Request.ToCurlAsync().GetAwaiter().GetResult();

            return this;
        }

        public HttpRequestBuilder AddAllowedServerLogEventAction(Func<ServerLogEvent, bool> action, string? label = null)
        {
            Context.ServerLogInspector.AddAllowedAction(action, label);
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
        public AssertBuilder<HttpResponseMessage> AssertWithStatus(HttpStatusCode statusCode)
        {
            return AssertWithStatus<HttpResponseMessage>(statusCode);
        }

        /// <summary>
        /// Starts asserting with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public AssertBuilder<TResponse> AssertWithStatus<TResponse>(HttpStatusCode statusCode)
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
            return AssertWithStatus<LTestProblemDetails>(statusCode);
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
            SetRequestUri();

            return new AssertBuilder<TResponse>(Context, _facade);
        }

        private void SetRequestUri()
        {
            var requestUri = _linkGeneratorService.GetRequestUri(
                _linkGeneratorContext.ActionName,
                _linkGeneratorContext.ControllerName,
                _linkGeneratorContext.UriValues);

            Context.Request.RequestUri = requestUri;
        }
    }
}