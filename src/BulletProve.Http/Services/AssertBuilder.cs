using BulletProve.Exceptions;
using BulletProve.Http.Models;
using BulletProve.Logging;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// A helper class for json http response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class AssertBuilder<TResponse>
        where TResponse : class
    {
        private readonly ServerScope _facade;
        private readonly HttpRequestManager _httpRequestManager;
        private readonly ITestLogger _logger;

        private readonly List<Action<TResponse>> _responseObjectAssertions = new();
        private readonly List<Action<HttpResponseMessage>> _responseMessageAssertions = new();
        private readonly List<Action<IReadOnlyCollection<ServerLogEvent>>> _serverLogAssertions = new();

        private Action<HttpStatusCode>? _statusCodeAssert;

        /// <summary>
        /// Gets the request context.
        /// </summary>
        public HttpRequestContext Context { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertBuilder{TResponse}"/> class.
        /// </summary>
        /// <param name="request">Http request message.</param>
        /// <param name="facade">Service provider.</param>
        /// <param name="label">Label to be logged.</param>
        public AssertBuilder(HttpRequestContext context, ServerScope facade)
        {
            Context = context;
            _facade = facade;
            _httpRequestManager = facade.GetRequiredService<HttpRequestManager>();
            _logger = facade.Logger;
        }

        /// <summary>
        /// Ensures that the status code of the response is in [200-299].
        /// </summary>
        public AssertBuilder<TResponse> EnsureSuccessStatusCode()
        {
            _statusCodeAssert = code => ((int)code).Should().BeInRange(200, 299);
            return this;
        }

        /// <summary>
        /// Runs assert login on response statuscode.
        /// </summary>
        /// <param name="statusCode">Expected status code.</param>
        public AssertBuilder<TResponse> AssertStatusCode(HttpStatusCode statusCode)
        {
            _statusCodeAssert = code => code.Should().Be(statusCode);

            return this;
        }

        /// <summary>
        /// Runs assert logic on the response.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        public AssertBuilder<TResponse> AssertResponseObject(Action<TResponse> assertAction)
        {
            if (assertAction != null)
            {
                _responseObjectAssertions.Add(assertAction);
            }

            return this;
        }

        /// <summary>
        /// Runs assert logic on the response message.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        public AssertBuilder<TResponse> AssertResponseMessage(Action<HttpResponseMessage> assertAction)
        {
            if (assertAction != null)
            {
                _responseMessageAssertions.Add(assertAction);
            }

            return this;
        }

        /// <summary>
        /// Runs assert logic on LogSniffer events.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        public AssertBuilder<TResponse> AssertServerLogs(Action<IReadOnlyCollection<ServerLogEvent>> assertAction)
        {
            if (assertAction != null)
            {
                _serverLogAssertions.Add(assertAction);
            }

            return this;
        }

        /// <summary>
        /// Executes the query and assertions.
        /// </summary>
        /// <returns></returns>
        public async Task<TResponse> ExecuteAsync()
        {
            using var loggerScope = _facade.Logger.Scope(Context.Label);
            var response = await _httpRequestManager.ExecuteRequestAsync(Context, _facade);

            RunStatusCodeAssert(response.StatusCode);
            RunAssertions(_responseMessageAssertions, response, "reponse message");
            RunAssertions(_serverLogAssertions, _facade.LogSniffer.GetServerLogs(), "LogSniffer");

            var responseObject = await GetResponseObjectAsync(response);
            RunAssertions(_responseObjectAssertions, responseObject, "reponse object");

            return responseObject;
        }

        /// <summary>
        /// Runs the status code assertion.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        private void RunStatusCodeAssert(HttpStatusCode statusCode)
        {
            if (_statusCodeAssert != null)
            {
                _statusCodeAssert(statusCode);
            }
            else
            {
                _logger.LogWarning($"{(int)statusCode} StatusCode was not checked");
            }
        }

        /// <summary>
        /// Gets the response object.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>A TResponse.</returns>
        private static async Task<TResponse> GetResponseObjectAsync(HttpResponseMessage response)
        {
            var responseType = typeof(TResponse);
            if (responseType == typeof(HttpResponseMessage))
            {
                return (response as TResponse)!;
            }

            var responseMessage = await response.Content.ReadAsStringAsync();
            var responseObject = TryDeserializeReponseMessage(responseMessage, responseType);
            response.Dispose();

            return responseObject;
        }

        /// <summary>
        /// Tries the deserialize reponse message.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="responseType">The response type.</param>
        /// <returns>A TResponse.</returns>
        private static TResponse TryDeserializeReponseMessage(string responseMessage, Type responseType)
        {
            if (responseType == typeof(EmptyResponse) && string.IsNullOrWhiteSpace(responseMessage))
            {
                return JsonConvert.DeserializeObject<TResponse>("{}")!;
            }
            else if (responseType == typeof(string))
            {
                return (responseMessage as TResponse)!;
            }

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(responseMessage, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                })!;
            }
            catch (Exception ex)
            {
                throw new BulletProveException($"Could not deserialize response message to '{typeof(TResponse).Name}'!", ex);
            }
        }

        /// <summary>
        /// Runs the assertions.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        private void RunAssertions<T>(List<Action<T>> assertions, T value, string message)
        {
            if (assertions.Count == 0)
            {
                return;
            }

            foreach (var assertion in assertions)
            {
                assertion(value);
            }

            _logger.LogInformation($"{assertions.Count} {message} assertions succeeded");
        }
    }
}