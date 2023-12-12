using BulletProve.Http.Filters;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BulletProve.Http.Tests.Filters
{
    /// <summary>
    /// The http request filter tests.
    /// </summary>
    public class HttpRequestFilter_Tests
    {
        private readonly RequestDelegate _sut;
        private bool _nextCalled;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestFilter_Tests"/> class.
        /// </summary>
        public HttpRequestFilter_Tests()
        {
            var services = new ServiceCollection();
            var builder = new ApplicationBuilder(services.BuildServiceProvider());
            Action<IApplicationBuilder> next = builder =>
            {
                _nextCalled = true;
            };

            var filter = new HttpRequestFilter();
            var action = filter.Configure(next);
            action(builder);

            _sut = builder.Build();
        }

        [Fact]
        public async Task TestConfigure()
        {
            var scope = Substitute.For<IDisposable>();
            var services = new ServiceCollection();
            services.AddSingleton(typeof(ILogger<>), typeof(TestLogger<>));
            var provider = services.BuildServiceProvider();

            var request = Substitute.For<HttpRequest>();
            var context = Substitute.For<HttpContext>();
            var headers = new HeaderDictionary
            {
                { Constants.BulletProveRequestID, "id" }
            };

            context.RequestServices.Returns(provider);
            request.Headers.Returns(headers);
            context.Request.Returns(request);
            await _sut(context);

            headers.Count.Should().Be(0);
            _nextCalled.Should().BeTrue();

            TestLogger<HttpRequestFilter>.State.Should().BeOfType<List<KeyValuePair<string, object>>>();
            var state = TestLogger<HttpRequestFilter>.State as List<KeyValuePair<string, object>>;
            state.Should().HaveCount(1);
            state![0].Key.Should().Be(Constants.BulletProveRequestID);
            state[0].Value.Should().Be("id");

            TestLogger<HttpRequestFilter>.Scope.Received(1).Dispose();
        }

        private class TestLogger<T> : ILogger<T>
        {
            public static object? State { get; private set; }
            public static IDisposable Scope { get; } = Substitute.For<IDisposable>();

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            {
                State = state;
                return Scope;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                throw new NotImplementedException();
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                throw new NotImplementedException();
            }
        }
    }
}
