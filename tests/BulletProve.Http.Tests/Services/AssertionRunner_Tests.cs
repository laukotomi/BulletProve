using BulletProve.Http.Services;
using BulletProve.Logging;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The assertion runner tests.
    /// </summary>
    public class AssertionRunner_Tests
    {
        private readonly ITestLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionRunner_Tests"/> class.
        /// </summary>
        public AssertionRunner_Tests()
        {
            _logger = Substitute.For<ITestLogger>();
        }

        /// <summary>
        /// Tests the run response message assertions without status code assertion.
        /// </summary>
        [Fact]
        public void TestRunResponseMessageAssertionsWithoutStatusCodeAssertion()
        {
            var sut = new AssertionRunner<Response>(_logger, null, [], [], []);
            sut.RunResponseMessageAssertions(new HttpResponseMessage());
            _logger.Received(1).LogWarning(Arg.Any<string>());
        }

        /// <summary>
        /// Tests the run response message assertions.
        /// </summary>
        [Fact]
        public void TestRunResponseMessageAssertions()
        {
            var sut = new AssertionRunner<Response>(
                _logger,
                sc => sc.Should().Be(HttpStatusCode.OK),
                [
                    rm => rm.IsSuccessStatusCode.Should().BeTrue()
                ],
                [],
                []);

            sut.RunResponseMessageAssertions(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

            _logger.Received(0).LogWarning(Arg.Any<string>());
            _logger.Received(1).LogInformation("1 response message assertions succeeded");
        }

        /// <summary>
        /// Tests the run server log assertions.
        /// </summary>
        [Fact]
        public void TestRunServerLogAssertions()
        {
            var sut = new AssertionRunner<Response>(
                _logger,
                null,
                [],
                [
                    logs => logs.Should().Contain(x => x.IsUnexpected)
                ],
                []);

            sut.RunServerLogAssertions([
                new ServerLogEvent("cat", LogLevel.Warning, new(), "msg", null, null)
                {
                    IsUnexpected = true
                }
            ]);

            _logger.Received(1).LogInformation("1 server log assertions succeeded");
        }

        [Fact]
        public void TestRunResponseObjectAssertions()
        {
            var sut = new AssertionRunner<Response>(
                _logger,
                null,
                [],
                [],
                [
                    x => x.Name.Should().Be("name")
                ]);

            sut.RunResponseObjectAssertions(new Response
            {
                Name = "name"
            });

            _logger.Received(1).LogInformation("1 response object assertions succeeded");
        }

        /// <summary>
        /// The response.
        /// </summary>
        private class Response
        {
            public required string Name { get; set; }
        }
    }
}
