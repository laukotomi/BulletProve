using BulletProve.ServerLog;
using FluentAssertions;
using System.Net;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The assertion builder.
    /// </summary>
    public class AssertionBuilder<TResponse>
        where TResponse : class
    {
        private readonly List<Action<TResponse>> _responseObjectAssertions = new();
        private readonly List<Action<HttpResponseMessage>> _responseMessageAssertions = new();
        private readonly List<Action<IReadOnlyCollection<ServerLogEvent>>> _serverLogAssertions = new();

        private Action<HttpStatusCode>? _statusCodeAssert;

        /// <summary>
        /// Ensures that the status code of the response is in [200-299].
        /// </summary>
        public AssertionBuilder<TResponse> EnsureSuccessStatusCode()
        {
            _statusCodeAssert = code => ((int)code).Should().BeInRange(200, 299);
            return this;
        }

        /// <summary>
        /// Runs assert login on response statuscode.
        /// </summary>
        /// <param name="statusCode">Expected status code.</param>
        public AssertionBuilder<TResponse> AssertStatusCode(HttpStatusCode statusCode)
        {
            _statusCodeAssert = code => code.Should().Be(statusCode);

            return this;
        }

        /// <summary>
        /// Runs assert logic on the response.
        /// </summary>
        /// <param name="assertAction">Assert action.</param>
        public AssertionBuilder<TResponse> AssertResponseObject(Action<TResponse> assertAction)
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
        public AssertionBuilder<TResponse> AssertResponseMessage(Action<HttpResponseMessage> assertAction)
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
        public AssertionBuilder<TResponse> AssertServerLogs(Action<IReadOnlyCollection<ServerLogEvent>> assertAction)
        {
            if (assertAction != null)
            {
                _serverLogAssertions.Add(assertAction);
            }

            return this;
        }

        /// <summary>
        /// Builds the assertion runner.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>An AssertionRunner.</returns>
        public AssertionRunner<TResponse> BuildAssertionRunner(ServerScope scope, HttpResponseMessage response)
        {
            return new(scope.Logger, _statusCodeAssert, _responseMessageAssertions, _serverLogAssertions, _responseObjectAssertions);
        }
    }
}