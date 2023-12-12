using BulletProve.ServerLog;
using FluentAssertions;
using System.Net;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The assertion builder.
    /// </summary>
    public class AssertionBuilder<TResponse> : IAssertionBuilder<TResponse>
        where TResponse : class
    {
        private readonly List<Action<TResponse>> _responseObjectAssertions = new();
        private readonly List<Action<HttpResponseMessage>> _responseMessageAssertions = new();
        private readonly List<Action<IReadOnlyCollection<ServerLogEvent>>> _serverLogAssertions = new();

        private Action<HttpStatusCode>? _statusCodeAssert;

        /// <inheritdoc/>
        public IAssertionBuilder<TResponse> AssertResponseMessage(Action<HttpResponseMessage> assertAction)
        {
            _responseMessageAssertions.Add(assertAction);
            return this;
        }

        /// <inheritdoc/>
        public IAssertionBuilder<TResponse> AssertResponseObject(Action<TResponse> assertAction)
        {
            _responseObjectAssertions.Add(assertAction);
            return this;
        }

        /// <inheritdoc/>
        public IAssertionBuilder<TResponse> AssertServerLogs(Action<IReadOnlyCollection<ServerLogEvent>> assertAction)
        {
            _serverLogAssertions.Add(assertAction);
            return this;
        }

        /// <inheritdoc/>
        public IAssertionBuilder<TResponse> AssertStatusCode(HttpStatusCode statusCode)
        {
            _statusCodeAssert = code => code.Should().Be(statusCode);
            return this;
        }

        /// <inheritdoc/>
        public IAssertionRunner<TResponse> BuildAssertionRunner(IServerScope scope)
        {
            return new AssertionRunner<TResponse>(scope.Logger, _statusCodeAssert, _responseMessageAssertions, _serverLogAssertions, _responseObjectAssertions);
        }

        /// <inheritdoc/>
        public IAssertionBuilder<TResponse> EnsureSuccessStatusCode()
        {
            _statusCodeAssert = code => ((int)code).Should().BeInRange(200, 299);
            return this;
        }
    }
}