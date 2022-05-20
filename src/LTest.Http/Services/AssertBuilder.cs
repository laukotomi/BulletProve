using FluentAssertions;
using LTest.Helpers;
using LTest.Http.Configuration;
using LTest.Http.Helpers;
using LTest.Http.Interfaces;
using LTest.Logging;
using LTest.LogSniffer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LTest.Http.Services
{
    /// <summary>
    /// A helper class for json http response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class AssertBuilder<TResponse>
        where TResponse : class
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _label;
        private readonly HttpConfiguration _httpConfiguration;
        private readonly ILogSnifferService _logSniffer;
        private readonly HttpClient _httpClient;
        private readonly ITestLogger _logger;

        private readonly List<Action<TResponse>> _responseObjectAssertions = new();
        private readonly List<Action<HttpResponseMessage>> _responseMessageAssertions = new();
        private readonly List<Action<IReadOnlyCollection<ServerLogEvent>>> _serverLogAssertions = new();

        private Action<HttpStatusCode> _statusCodeAssert;

        /// <summary>
        /// Gets the request.
        /// </summary>
        public HttpRequestMessage Request { get; } = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertBuilder{TResponse}"/> class.
        /// </summary>
        /// <param name="request">Http request message.</param>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="label">Label to be logged.</param>
        public AssertBuilder(
            HttpRequestMessage request,
            IServiceProvider serviceProvider,
            string label)
        {
            Request = request;
            _serviceProvider = serviceProvider;
            _label = label;
            _httpConfiguration = serviceProvider.GetRequiredService<HttpConfiguration>();
            _logSniffer = serviceProvider.GetRequiredService<ILogSnifferService>();
            _httpClient = serviceProvider.GetRequiredService<LTestHttpClientAccessor>().Client;
            _logger = serviceProvider.GetRequiredService<ITestLogger>();
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
            await HookHelper.RunHooksAsync<IBeforeHttpRequestHook>(_serviceProvider, x => x.BeforeHttpRequestAsync(_label, Request));

            using var loggerScope = _logger.Scope(logger => logger.LogInformation(LogHelper.CreateRequestLog(Request, _httpClient, _httpConfiguration)));
            var result = await StopwatchHelper.MeasureAsync(() => _httpClient.SendAsync(Request));
            var response = result.ResultObject;

            await HookHelper.RunHooksAsync<IAfterHttpRequestHook>(_serviceProvider, x => x.AfterHttpRequestAsync(_label, response));

            Request.Dispose();

            _logger.LogInformation(LogHelper.CreateResponseLog(response, result.ElapsedMilliseconds, _httpConfiguration));

            RunStatusCodeAssert(response.StatusCode);
            RunAssertions(_responseMessageAssertions, response, "reponse message");
            RunAssertions(_serverLogAssertions, _logSniffer.GetServerLogs(), "LogSniffer");

            var responseObject = GetResponseObject(response);
            RunAssertions(_responseObjectAssertions, responseObject, "reponse object");

            loggerScope.Finish(logger => logger.LogInformation($"Request '{_label}' executed"));

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
                _logger.LogInformation($"{(int)statusCode} StatusCode checked");
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
        private static TResponse GetResponseObject(HttpResponseMessage response)
        {
            if (typeof(TResponse) == typeof(HttpResponseMessage))
            {
                return response as TResponse;
            }

            var responseMessage = response.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseObject = TryDeserializeReponseMessage(responseMessage);
            response.Dispose();

            return responseObject;
        }

        /// <summary>
        /// Tries the deserialize reponse message.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <returns>A TResponse.</returns>
        private static TResponse TryDeserializeReponseMessage(string responseMessage)
        {
            if (typeof(TResponse) == typeof(EmptyResponse) && string.IsNullOrWhiteSpace(responseMessage))
            {
                return JsonConvert.DeserializeObject<TResponse>("{}");
            }
            else if (typeof(TResponse) == typeof(string))
            {
                return responseMessage as TResponse;
            }

            try
            {
                return JsonConvert.DeserializeObject<TResponse>(responseMessage, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Could not deserialize response message to '{typeof(TResponse).Name}'!", ex);
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