using FluentAssertions;
using LTest.Helpers;
using LTest.Http.Configuration;
using LTest.Http.Helpers;
using LTest.Http.Interfaces;
using LTest.Interfaces;
using LTest.LogSniffer;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace LTest.Http.Services
{
    /// <summary>
    /// A helper class for json http response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public class AssertBuilder<TResponse>
        where TResponse : class
    {
        private readonly HttpRequestMessage _request;
        private readonly IServiceProvider _serviceProvider;
        private readonly bool _autoRestoreUnexpectedLogSnifferEventAction;
        private readonly string _label;
        private readonly HttpConfiguration _httpConfiguration;
        private readonly HttpClient _httpClient;
        private readonly ILogSnifferService _logSnifferService;
        private readonly ITestLogger _logger;
        private readonly List<Action<TResponse>> _responseObjectAssertions = new();
        private readonly List<Action<HttpResponseMessage>> _responseMessageAssertions = new();
        private readonly List<Action<List<LogSnifferEvent>>> _logSnifferEventsAssertions = new();

        private Action<HttpStatusCode> _statusCodeAssert;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertBuilder{TResponse}"/> class.
        /// </summary>
        /// <param name="request">Http request message.</param>
        /// <param name="serviceProvider">Service provider.</param>
        /// <param name="restoreUnexpectedLogSnifferEventAction">Restore LogSniffer.</param>
        /// <param name="label">Label to be logged.</param>
        public AssertBuilder(
            HttpRequestMessage request,
            IServiceProvider serviceProvider,
            bool restoreUnexpectedLogSnifferEventAction,
            string label)
        {
            _request = request;
            _serviceProvider = serviceProvider;
            _autoRestoreUnexpectedLogSnifferEventAction = restoreUnexpectedLogSnifferEventAction;
            _label = label;
            _httpConfiguration = serviceProvider.GetRequiredService<HttpConfiguration>();
            _httpClient = serviceProvider.GetRequiredService<HttpClientAccessor>().Client;
            _logSnifferService = serviceProvider.GetRequiredService<ILogSnifferService>();
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
        public AssertBuilder<TResponse> AssertLogSnifferEvents(Action<List<LogSnifferEvent>> assertAction)
        {
            if (assertAction != null)
            {
                _logSnifferEventsAssertions.Add(assertAction);
            }

            return this;
        }

        /// <summary>
        /// Executes the query and assertions.
        /// </summary>
        /// <returns></returns>
        public TResponse Execute()
        {
            using var loggerScope = _logger.Scope(logger => logger.Info(LogHelper.CreateRequestLog(_request, _label, _httpClient, _httpConfiguration)));
            using var response = StopwatchHelper.MeasureMilliseconds(() => _httpClient.SendAsync(_request).GetAwaiter().GetResult(), out var elapsedMs);

            _request.Dispose();
            _logger.Info(LogHelper.CreateResponseLog(response, elapsedMs, _httpConfiguration));

            if (_statusCodeAssert != null)
            {
                _statusCodeAssert(response.StatusCode);
                _logger.Info($"{(int)response.StatusCode} StatusCode checked");
            }

            RunAssertions(_responseMessageAssertions, response, "reponse message");

            var responseMessage = response.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            var responseObject = TryDeserializeReponseMessage(responseMessage);

            RunAssertions(_responseObjectAssertions, responseObject, "reponse object");

            if (_autoRestoreUnexpectedLogSnifferEventAction)
            {
                _logSnifferService.OverrideIsExpectedLogEventAction(null);
            }

            RunAssertions(_logSnifferEventsAssertions, _logSnifferService.GetSnapshot(), "LogSniffer");
            loggerScope.Finish(logger => logger.Info($"Request executed"));

            ServicesHelper.RunServices<IAfterHttpRequestBehavior>(_serviceProvider);

            return responseObject;
        }

        /// <summary>
        /// Generates curl from the request.
        /// </summary>
        public string ToCurl()
        {
            return _request.ToCurl();
        }

        private TResponse TryDeserializeReponseMessage(string responseMessage)
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

            _logger.Info($"{assertions.Count} {message} assertions succeeded");
        }
    }
}