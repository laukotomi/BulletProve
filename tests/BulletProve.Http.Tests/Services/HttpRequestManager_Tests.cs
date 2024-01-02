using BulletProve.Base.Configuration;
using BulletProve.Base.Hooks;
using BulletProve.Http.Configuration;
using BulletProve.Http.Interfaces;
using BulletProve.Http.Models;
using BulletProve.Http.Services;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The http request manager tests.
    /// </summary>
    public class HttpRequestManager_Tests
    {
        private readonly IHookRunner _hookRunner;
        private readonly HttpRequestManager _sut;
        private readonly HttpRequestContext _context;
        private readonly ServerScope _scope;
        private readonly Handler _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestManager_Tests"/> class.
        /// </summary>
        public HttpRequestManager_Tests()
        {
            _hookRunner = Substitute.For<IHookRunner>();
            _sut = new HttpRequestManager(_hookRunner);

            _context = new HttpRequestContext("label");
            _context.Request.RequestUri = new Uri("https://localhost/");

            _handler = new Handler(_context, _sut);
            var client = new HttpClient(_handler);

            var services = new ServiceCollection();
            services.AddSingleton(new HttpConfiguration());
            services.AddSingleton<ITestLogger, TestLogger>();
            services.AddSingleton<ScopeProvider>();
            services.AddSingleton<DisposableCollector>();
            services.AddSingleton<IServerLogCollector, ServerLogCollector>();
            services.AddSingleton(new ServerConfigurator());

            _scope = new ServerScope(services.BuildServiceProvider(), client);

            var sp = _scope.GetRequiredService<ScopeProvider>();
            var logScope = sp.CreateScope("cat", new List<KeyValuePair<string, object>>
            {
                KeyValuePair.Create<string, object>(Constants.BulletProveRequestID, "label")
            });
            _handler.Log = new("cat", LogLevel.Error, new(), "msg", (Scope)logScope, null);
        }

        /// <summary>
        /// Tests the execute request async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestExecuteRequestAsync()
        {
            var response = await _sut.ExecuteRequestAsync(_context, _scope);

            _context.Request.Headers.Should().Contain(x => x.Key == Constants.BulletProveRequestID);
            _context.Request.Headers.First(x => x.Key == Constants.BulletProveRequestID).Value.First().Should().Be("label");

            await _hookRunner.Received(1).RunHooksAsync(Arg.Any<Func<IBeforeHttpRequestHook, Task>>());
            await _hookRunner.Received(1).RunHooksAsync(Arg.Any<Func<IAfterHttpRequestHook, Task>>());

            response.Should().Be(_handler.Response);
        }

        /// <summary>
        /// Tests the handle server log.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestHandleServerLog()
        {
            _handler.SendServerLog = true;
            await _sut.ExecuteRequestAsync(_context, _scope);

            var logs = _context.Logs.GetServerLogs();
            logs.Should().HaveCount(1);
            logs[0].Should().Be(_handler.Log);
        }

        /// <summary>
        /// Tests the handle server log without request.
        /// </summary>
        [Fact]
        public void TestHandleServerLogWithoutRequest()
        {
            _sut.HandleServerLog(_handler.Log);
            var logs = _context.Logs.GetServerLogs();
            logs.Should().HaveCount(0);
        }

        /// <summary>
        /// Tests the is allowed.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestIsAllowed()
        {
            _handler.SendIsAllowed = true;
            _context.ServerLogInspector.AddAllowedAction(x => true);
            await _sut.ExecuteRequestAsync(_context, _scope);

            _handler.IsAllowedResult.Should().BeTrue();
        }

        /// <summary>
        /// Tests the is allowed without request.
        /// </summary>
        [Fact]
        public void TestIsAllowedWithoutRequest()
        {
            var isAllowed = _sut.IsAllowed(_handler.Log);
            isAllowed.Should().BeFalse();
        }

        /// <summary>
        /// The handler.
        /// </summary>
        private class Handler : HttpMessageHandler
        {
            private readonly HttpRequestContext _context;
            private readonly HttpRequestManager _sut;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="sut">The sut.</param>
            public Handler(HttpRequestContext context, HttpRequestManager sut)
            {
                _context = context;
                _sut = sut;
            }

            /// <summary>
            /// Gets the response.
            /// </summary>
            public HttpResponseMessage Response { get; } = new();

            /// <summary>
            /// Gets or sets a value indicating whether send server log.
            /// </summary>
            public bool SendServerLog { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether send is allowed.
            /// </summary>
            public bool SendIsAllowed { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether allowed is result.
            /// </summary>
            public bool? IsAllowedResult { get; set; }

            /// <summary>
            /// Gets or sets the log.
            /// </summary>
            public ServerLogEvent Log { get; set; } = null!;

            /// <inheritdoc/>
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (SendServerLog)
                    _sut.HandleServerLog(Log);

                if (SendIsAllowed)
                    IsAllowedResult = _sut.IsAllowed(Log);

                request.Should().Be(_context.Request);
                return Task.FromResult(Response);
            }
        }
    }
}
