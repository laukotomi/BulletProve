using BulletProve.Logging;
using BulletProve.ServerLog;
using System.Net;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The assertion runner.
    /// </summary>
    public class AssertionRunner<TResponse> : IAssertionRunner<TResponse>
        where TResponse : class
    {
        private readonly ITestLogger _logger;

        private readonly Action<HttpStatusCode>? _statusCodeAssertion;
        private readonly IReadOnlyCollection<Action<HttpResponseMessage>> _responseMessageAssertions;
        private readonly IReadOnlyCollection<Action<IReadOnlyCollection<ServerLogEvent>>> _serverLogAssertions;
        private readonly IReadOnlyCollection<Action<TResponse>> _responseObjectAssertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionRunner"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="statusCodeAssertion">The status code assertion.</param>
        /// <param name="responseMessageAssertions">The response message assertions.</param>
        /// <param name="serverLogAssertions">The server log assertions.</param>
        /// <param name="responseObjectAssertions">The response object assertions.</param>
        public AssertionRunner(
            ITestLogger logger,
            Action<HttpStatusCode>? statusCodeAssertion,
            List<Action<HttpResponseMessage>> responseMessageAssertions,
            List<Action<IReadOnlyCollection<ServerLogEvent>>> serverLogAssertions,
            List<Action<TResponse>> responseObjectAssertions)
        {
            _logger = logger;
            _statusCodeAssertion = statusCodeAssertion;
            _responseMessageAssertions = responseMessageAssertions;
            _serverLogAssertions = serverLogAssertions;
            _responseObjectAssertions = responseObjectAssertions;
        }

        /// <inheritdoc/>
        public void RunResponseMessageAssertions(HttpResponseMessage response)
        {
            if (_statusCodeAssertion != null)
            {
                _statusCodeAssertion(response.StatusCode);
            }
            else
            {
                _logger.LogWarning($"{(int)response.StatusCode} StatusCode was not checked");
            }

            RunAssertions(_responseMessageAssertions, response, "response message");
        }

        /// <inheritdoc/>
        public void RunServerLogAssertions(IReadOnlyCollection<ServerLogEvent> serverLogs)
        {
            RunAssertions(_serverLogAssertions, serverLogs, "server log");
        }

        /// <inheritdoc/>
        public void RunResponseObjectAssertions(TResponse responseObject)
        {
            RunAssertions(_responseObjectAssertions, responseObject, "response object");
        }

        /// <summary>
        /// Runs the assertions.
        /// </summary>
        /// <param name="assertions">The assertions.</param>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        private void RunAssertions<T>(IReadOnlyCollection<Action<T>> assertions, T value, string message)
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
