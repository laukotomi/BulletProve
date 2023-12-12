using BulletProve.Logging;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The logger provider tests.
    /// </summary>
    public class LoggerProvider_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Category";

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "Message";

        private readonly ServerLogHandler _handler;
        private readonly LoggerProvider _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerProvider_Tests"/> class.
        /// </summary>
        public LoggerProvider_Tests()
        {
            _handler = new ServerLogHandler();
            _sut = new LoggerProvider(Category, new ScopeProvider(), [_handler]);
        }

        /// <summary>
        /// Tests the is enabled.
        /// </summary>
        [Fact]
        public void TestIsEnabled()
        {
            foreach (var value in Enum.GetValues<LogLevel>())
            {
                _sut.IsEnabled(value).Should().BeTrue();
            }
        }

        /// <summary>
        /// Tests the log level and expected.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="isExpected">If true, is expected.</param>
        [Theory]
        [InlineData(LogLevel.Critical, false)]
        [InlineData(LogLevel.Warning, true)]
        public void TestLogLevelAndExpected(LogLevel logLevel, bool isExpected)
        {
            _handler.IsExpected = isExpected;
            _sut.Log(logLevel, Message);

            _handler.Events.Should().HaveCount(1);
            var logEvent = _handler.Events[0];

            logEvent.CategoryName.Should().Be(Category);
            logEvent.EventId.Should().NotBe(default);
            logEvent.Exception.Should().BeNull();
            logEvent.IsUnexpected.Should().Be(!isExpected);
            logEvent.Level.Should().Be(logLevel);
            logEvent.Message.Should().Be(Message);
            logEvent.Scope.Should().BeNull();
        }

        /// <summary>
        /// Tests the log with exception.
        /// </summary>
        [Fact]
        public void TestLogWithException()
        {
            _sut.Log(LogLevel.Error, new InvalidOperationException("ExceptionMessage"), Message);

            _handler.Events.Should().HaveCount(1);
            var logEvent = _handler.Events[0];

            logEvent.Exception.Should().NotBeNull();
            logEvent.Message.Should().Contain("ExceptionMessage");
        }

        [Fact]
        public void TestBeginScope()
        {
            using var scope = _sut.BeginScope("state");
            _sut.LogInformation(Message);

            _handler.Events.Should().HaveCount(1);
            var logEvent = _handler.Events[0];

            scope.Should().NotBeNull();
            logEvent.Scope.Should().Be(scope);
        }

        /// <summary>
        /// The server log handler.
        /// </summary>
        private class ServerLogHandler : IServerLogHandler
        {
            /// <summary>
            /// Gets or sets a value indicating whether is expected.
            /// </summary>
            public bool IsExpected { get; set; }

            /// <summary>
            /// Gets the events.
            /// </summary>
            public List<ServerLogEvent> Events { get; } = new();

            /// <inheritdoc/>
            public void HandleServerLog(ServerLogEvent serverLogEvent)
            {
                Events.Add(serverLogEvent);
            }

            /// <inheritdoc/>
            public bool IsAllowed(ServerLogEvent item)
            {
                return IsExpected;
            }
        }
    }
}
