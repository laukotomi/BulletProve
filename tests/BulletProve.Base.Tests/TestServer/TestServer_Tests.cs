using BulletProve.Exceptions;
using BulletProve.Hooks;
using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.Services;
using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BulletProve.Base.Tests.TestServer
{
    /// <summary>
    /// The test server tests.
    /// </summary>
    public class TestServer_Tests
    {
        /// <summary>
        /// The server name.
        /// </summary>
        private const string ServerName = "server";

        private readonly TestHooks _hooks;
        private readonly TestServer<Startup> _sut;
        private ServerConfigurator? _configurator;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServer_Tests"/> class.
        /// </summary>
        public TestServer_Tests()
        {
            _hooks = new TestHooks();

            _sut = new TestTestServer(x =>
            {
                x.MinimumLogLevel = LogLevel.Error;
                x.AddAppSetting("key", "value");
                x.AddJsonConfigurationFile("AppConfig.json");
                x.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IAfterServerStartedHook>(_hooks);
                    services.AddSingleton<IBeforeTestHook>(_hooks);
                    services.AddSingleton<IAfterTestHook>(_hooks);
                });
                _configurator = x;
            });
        }

        /// <summary>
        /// Tests the start session async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestStartSessionAsync()
        {
            var scope = await _sut.StartSessionAsync(ServerName);

            // Assert InitConfigurator
            _configurator.Should().NotBeNull();
            _configurator!.AppSettings.Should().ContainKey("https_port");
            _configurator.AppSettings["https_port"].Should().Be("443");
            _configurator.ServiceConfigurators.Count.Should().Be(2);
            _configurator.LoggerCategoryNameInspector
                .IsAllowed(typeof(Startup).Namespace!)
                .Should().BeTrue();
            _configurator.ServerLogInspector
                .IsAllowed(new ServerLogEvent("cat", LogLevel.Warning, new EventId(), "msg", null, null))
                .Should().BeFalse();
            _configurator.MinimumLogLevel.Should().Be(LogLevel.Error);

            // Assert StartServer
            Startup.ConfigureCalled.Should().BeTrue();
            var logger = (TestLogger)scope.Logger;
            var logs = logger.GetSnapshot();
            logs.All(x => x.Scope?.State!.Equals(ServerName) ?? false).Should().BeTrue();
            logs.Any(x => x.Message == "Starting server").Should().BeTrue();
            logs.Any(x => x.Message.StartsWith("Server started")).Should().BeTrue();

            // Assert ConfigureWebHost
            var config = scope.GetRequiredService<IConfiguration>();
            config["key"].Should().Be("value");
            config["var"].Should().Be("123");

            scope.GetRequiredService<ILoggerFactory>()
                .Should().BeOfType<TestLoggerFactory>();
            scope.GetService<ILoggerProvider>().Should().BeNull();

            // Assert RegisterTestServices
            scope.GetService<ServerConfigurator>().Should().NotBeNull().And.Be(_configurator);
            scope.GetService<ScopeProvider>().Should().NotBeNull();
            scope.GetService<ITestLogger>().Should().NotBeNull().And.BeOfType<TestLogger>();
            scope.GetService<IServerLogCollector>().Should().NotBeNull().And.BeOfType<ServerLogCollector>();
            scope.GetService<IServerLogHandler>().Should().NotBeNull().And.BeOfType<DefaultServerLogHandler>();
            scope.GetService<DisposableCollector>().Should().NotBeNull();
            scope.GetService<HookRunner>().Should().NotBeNull();

            // Assert RegisterResetSingletonHooks
            scope.GetServices<ICleanUpHook>().Count().Should().Be(7);

            // Assert StartSessionAsync
            _hooks.AfterServerStartedCalled.Should().BeTrue();
            _hooks.BeforeTestCalled.Should().BeTrue();
        }

        /// <summary>
        /// Tests the start server called twice.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestStartServerCalledTwice()
        {
            var scope = await _sut.StartSessionAsync(ServerName);
            _configurator = null;
            _hooks.Reset();
            await ((TestLogger)scope.Logger).CleanUpAsync();

            scope = await _sut.StartSessionAsync(ServerName);
            _configurator.Should().BeNull();

            var logs = ((TestLogger)scope.Logger).GetSnapshot();
            logs.Count.Should().Be(1);
            logs[0].Message.Should().Be("Server is already running");
            logs[0].Scope!.State.Should().Be(ServerName);

            _hooks.AfterServerStartedCalled.Should().BeFalse();
            _hooks.BeforeTestCalled.Should().BeTrue();
        }

        /// <summary>
        /// Tests the i clean up hook not singleton.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestICleanUpHookNotSingleton()
        {
            var sut = new TestTestServer(x =>
            {
                x.ConfigureTestServices(services =>
                {
                    services.AddTransient<ICleanUpHook, TestHooks>();
                });
            });

            var act = async () => await sut.StartSessionAsync(ServerName);
            await act.Should().ThrowAsync<BulletProveException>();
        }

        /// <summary>
        /// Tests the end session async without start session async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestEndSessionAsyncWithoutStartSessionAsync()
        {
            var output = Substitute.For<IOutput>();
            var sut = new TestTestServer(null);

            await sut.EndSessionAsync(output);

            output.Received(0).WriteLine(Arg.Any<string>());
        }

        /// <summary>
        /// Tests the end session async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestEndSessionAsync()
        {
            var output = Substitute.For<IOutput>();

            var scope = await _sut.StartSessionAsync(ServerName);
            await _sut.EndSessionAsync(output);

            _hooks.AfterTestCalled.Should().BeTrue();
            _hooks.CleanUpCalled.Should().BeTrue();

            output.Received(4).WriteLine(Arg.Any<string>());

            var act = () => scope.GetService<ITestLogger>();
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the open scopes.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestOpenScopes()
        {
            var output = Substitute.For<IOutput>();
            var scope = await _sut.StartSessionAsync(ServerName);

            var logScope = scope.Logger.Scope("state");
            var act = async () => await _sut.EndSessionAsync(output);
            await act.Should().ThrowAsync<BulletProveException>();
            logScope.Dispose();

            // scope should be disposed anyway
            var act2 = () => scope.GetService<ITestLogger>();
            act2.Should().Throw<Exception>();
        }

        /// <summary>
        /// Tests the unexpected server logs.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestUnexpectedServerLogs()
        {
            var output = Substitute.For<IOutput>();
            var scope = await _sut.StartSessionAsync(ServerName);

            scope.ServerLogCollector.AddServerLog(new ServerLogEvent("cat", LogLevel.Critical, new EventId(), "msg", null, null)
            {
                IsUnexpected = true
            });

            var act = async () => await _sut.EndSessionAsync(output);
            await act.Should().ThrowAsync<BulletProveException>();
        }

        /// <summary>
        /// Tests the flush logger.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestFlushLogger()
        {
            var logs = new List<string>();
            var output = Substitute.For<IOutput>();
            output.WriteLine(Arg.Do<string>(text => logs.Add(text)));
            var scope = await _sut.StartSessionAsync(ServerName);

            using (var s = scope.Logger.Scope("scope1"))
            {
                using var s2 = scope.Logger.Scope(new List<KeyValuePair<string, object>>
                {
                    KeyValuePair.Create<string, object>(Constants.BulletProveRequestID, "id1")
                });

                scope.Logger.LogInformation("Info");
                scope.Logger.LogError("Error");
            }

            scope.Logger.LogEmptyLine();
            scope.Logger.Log(new TestLogEvent("cat", LogLevel.Warning, "msg", false, null));

            await _sut.EndSessionAsync(output);
            logs.Count.Should().Be(11);
            logs[0].Should().Be("SCO: server");
            logs[1].Should().Be("T I: Starting server");
            logs[3].Should().BeEmpty();
            logs[4].Should().Be("SCO: scope1");
            logs[5].Should().Be("T I:   Info");
            logs[6].Should().Be("T E:   Error");
            logs[8].Should().BeEmpty();
            logs[9].Should().Be("SUW: msg");
        }

        /// <summary>
        /// The startup.
        /// </summary>
        private class Startup
        {
            /// <summary>
            /// Gets a value indicating whether configure called.
            /// </summary>
            public static bool ConfigureCalled { get; private set; }

            /// <summary>
            /// Configures the.
            /// </summary>
            public void Configure()
            {
                ConfigureCalled = true;
            }
        }

        /// <summary>
        /// The test test server.
        /// </summary>
        private class TestTestServer(Action<ServerConfigurator>? configAction)
            : TestServer<Startup>(configAction)
        {
            /// <inheritdoc/>
            protected override IWebHostBuilder? CreateWebHostBuilder()
            {
                var builder = new WebHostBuilder();
                builder.UseStartup<Startup>();
                builder.UseEnvironment(Environments.Development);
                return builder;
            }

            /// <inheritdoc/>
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                base.ConfigureWebHost(builder);

                builder.UseContentRoot(Directory.GetCurrentDirectory());
            }
        }

        /// <summary>
        /// The test hooks.
        /// </summary>
        private class TestHooks : IAfterServerStartedHook, IBeforeTestHook, IAfterTestHook, ICleanUpHook
        {
            /// <summary>
            /// Gets a value indicating whether after server started called.
            /// </summary>
            public bool AfterServerStartedCalled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether before test called.
            /// </summary>
            public bool BeforeTestCalled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether after test called.
            /// </summary>
            public bool AfterTestCalled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether clean up called.
            /// </summary>
            public bool CleanUpCalled { get; private set; }

            /// <inheritdoc/>
            public Task AfterServerStartedAsync()
            {
                AfterServerStartedCalled = true;
                return Task.CompletedTask;
            }

            /// <inheritdoc/>
            public Task AfterTestAsync()
            {
                AfterTestCalled = true;
                return Task.CompletedTask;
            }

            /// <inheritdoc/>
            public Task BeforeTestAsync()
            {
                BeforeTestCalled = true;
                return Task.CompletedTask;
            }

            /// <inheritdoc/>
            public Task CleanUpAsync()
            {
                CleanUpCalled = true;
                return Task.CompletedTask;
            }

            /// <summary>
            /// Resets the state.
            /// </summary>
            public void Reset()
            {
                AfterServerStartedCalled = false;
                BeforeTestCalled = false;
                CleanUpCalled = false;
                AfterTestCalled = false;
            }
        }
    }
}
