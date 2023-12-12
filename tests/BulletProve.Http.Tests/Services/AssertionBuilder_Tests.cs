using BulletProve.Http.Services;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The assertion builder tests.
    /// </summary>
    public class AssertionBuilder_Tests
    {
        private readonly IServerScope _scope;
        private readonly AssertionBuilder<Response> _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionBuilder_Tests"/> class.
        /// </summary>
        public AssertionBuilder_Tests()
        {
            _scope = Substitute.For<IServerScope>();
            _sut = new AssertionBuilder<Response>();
        }

        /// <summary>
        /// Tests the assert response message.
        /// </summary>
        [Fact]
        public void TestAssertResponseMessage()
        {
            _sut.AssertResponseMessage(x => x.Headers.Should().Contain(h => h.Key == "Pragma"));
            var runner = _sut.BuildAssertionRunner(_scope);

            var response = new HttpResponseMessage();

            var act = () => runner.RunResponseMessageAssertions(response);
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the assert response object.
        /// </summary>
        [Fact]
        public void TestAssertResponseObject()
        {
            _sut.AssertResponseObject(x => x.Name.Should().Be("asd"));
            var runner = _sut.BuildAssertionRunner(_scope);

            var response = new Response
            {
                Name = "name"
            };

            var act = () => runner.RunResponseObjectAssertions(response);
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the assert server logs.
        /// </summary>
        [Fact]
        public void TestAssertServerLogs()
        {
            _sut.AssertServerLogs(x => x.Should().Contain(l => l.Level == LogLevel.Warning));
            var runner = _sut.BuildAssertionRunner(_scope);

            var logs = new List<ServerLogEvent>();
            var act = () => runner.RunServerLogAssertions(logs);
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the assert status code.
        /// </summary>
        [Fact]
        public void TestAssertStatusCode()
        {
            _sut.AssertStatusCode(System.Net.HttpStatusCode.OK);
            var runner = _sut.BuildAssertionRunner(_scope);

            var response = new HttpResponseMessage()
            {
                StatusCode = System.Net.HttpStatusCode.OK
            };
            var act = () => runner.RunResponseMessageAssertions(response);
            act.Should().NotThrow<Exception>();
        }

        /// <summary>
        /// Tests the ensure success status code.
        /// </summary>
        [Fact]
        public void TestEnsureSuccessStatusCode()
        {
            _sut.EnsureSuccessStatusCode();
            var runner = _sut.BuildAssertionRunner(_scope);
            var response = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.NotFound
            };
            var act = () => runner.RunResponseMessageAssertions(response);
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// The response.
        /// </summary>
        private class Response
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public required string Name { get; set; }
        }
    }
}
