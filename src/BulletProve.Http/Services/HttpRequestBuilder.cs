using BulletProve.Http.Configuration;
using BulletProve.Http.Models;
using BulletProve.ServerLog;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// Helper class to build and send the HTTP request.
    /// </summary>
    public class HttpRequestBuilder
    {
        private readonly LinkGeneratorContext _linkGeneratorContext;
        private readonly ServerScope _scope;
        private readonly LinkGeneratorService _linkGeneratorService;
        private readonly HttpRequestManager _httpRequestManager;
        private ResponseMessageDeserializer _responseMessageDeserializer;

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public HttpRequestContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class.
        /// </summary>
        /// <param name="linkGeneratorContext">Link generator context.</param>
        /// <param name="scope">Server scope.</param>
        public HttpRequestBuilder(LinkGeneratorContext linkGeneratorContext, ServerScope scope)
        {
            _linkGeneratorContext = linkGeneratorContext;
            _scope = scope;

            Context = new HttpRequestContext(scope.GetRequiredService<LabelGeneratorService>().GetLabel());
            Context.Request.Method = _linkGeneratorContext.Method;
            _scope.DisposableCollertor.Add(Context);
            _linkGeneratorService = _scope.GetRequiredService<LinkGeneratorService>();
            _httpRequestManager = _scope.GetRequiredService<HttpRequestManager>();
            _responseMessageDeserializer = _scope.GetRequiredService<HttpConfiguration>().ResponseMessageDeserializer;
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
        /// Sets the bearer token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A HttpRequestBuilder.</returns>
        public HttpRequestBuilder SetBearerToken(string token)
        {
            Context.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
            _scope.DisposableCollertor.Add(stringContent);
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
        public HttpRequestBuilder SetLabel(string label)
        {
            if (string.IsNullOrEmpty(label))
            {
                throw new ArgumentNullException(nameof(label));
            }

            Context.Label = label;
            return this;
        }

        /// <summary>
        /// Exports the request to curl.
        /// </summary>
        /// <param name="curl">The curl.</param>
        public HttpRequestBuilder ExportToCurl(out string curl)
        {
            SetRequestUri();
            curl = Context.Request.ToCurlAsync().GetAwaiter().GetResult();

            return this;
        }

        /// <summary>
        /// Adds an allowed server log event action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="label">The label.</param>
        public HttpRequestBuilder AddAllowedServerLogEvent(Func<ServerLogEvent, bool> action, string? label = null)
        {
            Context.ServerLogInspector.AddAllowedAction(action, label);
            return this;
        }

        /// <summary>
        /// Sets the response message deserializer.
        /// </summary>
        /// <param name="responseMessageDeserializer">The response message deserializer.</param>
        /// <returns>A HttpRequestBuilder.</returns>
        public HttpRequestBuilder SetResponseMessageDeserializer(ResponseMessageDeserializer responseMessageDeserializer)
        {
            _responseMessageDeserializer = responseMessageDeserializer;
            return this;
        }

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
        public Task<TResponse> ExecuteSuccessAsync<TResponse>(Action<AssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return ExecuteRequestAsync<TResponse>(assert =>
            {
                assert.EnsureSuccessStatusCode();
                assertionAction?.Invoke(assert);
            });
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
        public Task<TResponse> ExecuteAssertingStatusAsync<TResponse>(HttpStatusCode statusCode, Action<AssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return ExecuteRequestAsync<TResponse>(assert =>
            {
                assert.AssertStatusCode(statusCode);
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Starts asserting problem details with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public Task<TestProblemDetails> ExecuteAssertingProblemAndStatusAsync(HttpStatusCode statusCode)
        {
            return ExecuteAssertingStatusAsync<TestProblemDetails>(statusCode);
        }
        #endregion

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="assertionAction">The assertion action.</param>
        /// <returns>A Task.</returns>
        public Task<HttpResponseMessage> ExecuteRequestAsync(Action<AssertionBuilder<HttpResponseMessage>> assertionAction)
        {
            return ExecuteRequestAsync<HttpResponseMessage>(assertionAction);
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="assertionAction">The assertion action.</param>
        /// <returns>A Task.</returns>
        public async Task<TResponse> ExecuteRequestAsync<TResponse>(Action<AssertionBuilder<TResponse>> assertionAction)
            where TResponse : class
        {
            using var loggerScope = _scope.Logger.Scope(Context.Label);

            var assertBuilder = new AssertionBuilder<TResponse>();
            assertionAction?.Invoke(assertBuilder);

            SetRequestUri();
            var response = await _httpRequestManager.ExecuteRequestAsync(Context, _scope);

            var assertionRunner = assertBuilder.BuildAssertionRunner(_scope, response);
            assertionRunner.RunResponseMessageAssertions(response);
            assertionRunner.RunServerLogAssertions(_scope.LogSniffer.GetServerLogs());

            var responseObject = await _responseMessageDeserializer.GetResponseObjectAsync<TResponse>(response);
            assertionRunner.RunResponseObjectAssertions(responseObject);

            return responseObject;
        }

        /// <summary>
        /// Sets the request uri.
        /// </summary>
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