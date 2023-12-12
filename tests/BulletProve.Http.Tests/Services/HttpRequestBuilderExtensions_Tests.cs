using BulletProve.Http.Models;
using BulletProve.Http.Services;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System.Net;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The http request builder extensions tests.
    /// </summary>
    public class HttpRequestBuilderExtensions_Tests : HttpRequestBuilder_Tests
    {
        private static IAssertionRunner<HttpResponseMessage> _assertionRunner2 = null!;
        private static IAssertionBuilder<HttpResponseMessage> _assertionBuilder2 = null!;
        private static IAssertionRunner<TestProblemDetails> _assertionRunner3 = null!;
        private static IAssertionBuilder<TestProblemDetails> _assertionBuilder3 = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilderExtensions_Tests"/> class.
        /// </summary>
        public HttpRequestBuilderExtensions_Tests() : base(RegisterServices)
        {

        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="services">The services.</param>
        private static void RegisterServices(ServiceCollection services)
        {
            _assertionRunner2 = Substitute.For<IAssertionRunner<HttpResponseMessage>>();
            _assertionBuilder2 = Substitute.For<IAssertionBuilder<HttpResponseMessage>>();
            _assertionBuilder2.BuildAssertionRunner(Arg.Any<IServerScope>()).Returns(_assertionRunner2);

            _assertionRunner3 = Substitute.For<IAssertionRunner<TestProblemDetails>>();
            _assertionBuilder3 = Substitute.For<IAssertionBuilder<TestProblemDetails>>();
            _assertionBuilder3.BuildAssertionRunner(Arg.Any<IServerScope>()).Returns(_assertionRunner3);

            services.AddTransient(x => _assertionBuilder2);
            services.AddTransient(x => _assertionBuilder3);
        }

        /// <summary>
        /// Tests the execute request async with http response message.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteRequestAsyncWithHttpResponseMessage()
        {
            bool invoked = false;

            await _sut.ExecuteRequestAsync(x =>
            {
                x.Should().Be(_assertionBuilder2);
                invoked = true;
            });

            invoked.Should().BeTrue();
            await _httpRequestManager.Received(1).ExecuteRequestAsync(_sut.Context, Arg.Any<IServerScope>());
            _assertionBuilder2.Received(1).BuildAssertionRunner(Arg.Any<IServerScope>());
            _assertionRunner2.Received(1).RunResponseMessageAssertions(_response);
            _assertionRunner2.Received(1).RunServerLogAssertions(Arg.Any<IReadOnlyCollection<ServerLogEvent>>());
            _assertionRunner2.Received(1).RunResponseObjectAssertions(_response);
        }

        /// <summary>
        /// Tests the execute success async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteSuccessAsync()
        {
            bool invoked = false;

            await _sut.ExecuteSuccessAsync(x =>
            {
                x.Should().Be(_assertionBuilder2);
                invoked = true;
            });

            invoked.Should().BeTrue();
            _assertionBuilder2.Received(1).EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Tests the execute success async generic.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteSuccessAsyncGeneric()
        {
            bool invoked = false;

            await _sut.ExecuteSuccessAsync<MyContent>(x =>
            {
                x.Should().Be(_assertionBuilder);
                invoked = true;
            });

            invoked.Should().BeTrue();
            _assertionBuilder.Received(1).EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Tests the execute asserting status async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteAssertingStatusAsync()
        {
            bool invoked = false;

            await _sut.ExecuteAssertingStatusAsync(HttpStatusCode.OK, x =>
            {
                x.Should().Be(_assertionBuilder2);
                invoked = true;
            });

            invoked.Should().BeTrue();
            _assertionBuilder2.Received(1).AssertStatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests the execute asserting status async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteAssertingStatusAsyncGeneric()
        {
            bool invoked = false;

            await _sut.ExecuteAssertingStatusAsync<MyContent>(HttpStatusCode.OK, x =>
            {
                x.Should().Be(_assertionBuilder);
                invoked = true;
            });

            invoked.Should().BeTrue();
            _assertionBuilder.Received(1).AssertStatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests the execute asserting problem and status async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteAssertingProblemAndStatusAsync()
        {
            bool invoked = false;

            await _sut.ExecuteAssertingProblemAndStatusAsync(HttpStatusCode.OK, x =>
            {
                x.Should().Be(_assertionBuilder3);
                invoked = true;
            });

            invoked.Should().BeTrue();
            _assertionBuilder3.Received(1).AssertStatusCode(HttpStatusCode.OK);
        }
    }
}
