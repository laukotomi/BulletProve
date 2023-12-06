using BulletProve.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The test logger tests.
    /// </summary>
    public class TestLoggerTests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "category";

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "message";

        private readonly TestLogger _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerTests"/> class.
        /// </summary>
        public TestLoggerTests()
        {
            var provider = new ScopeProvider();
            _sut = new TestLogger(provider);
        }

        /// <summary>
        /// Tests the initialization.
        /// </summary>
        [Fact]
        public void TestInitialization()
        {
            AssertEmpty(_sut);
        }

        /// <summary>
        /// Tests the log error.
        /// </summary>
        [Fact]
        public void TestLogError()
        {
            _sut.LogError("Error");

            AssertLogs(_sut, LogLevel.Error, "Error");
        }

        /// <summary>
        /// Tests the log warning.
        /// </summary>
        [Fact]
        public void TestLogWarning()
        {
            _sut.LogWarning("Warning");

            AssertLogs(_sut, LogLevel.Warning, "Warning");
        }

        /// <summary>
        /// Tests the log information.
        /// </summary>
        [Fact]
        public void TestLogInformation()
        {
            _sut.LogInformation("Info");

            AssertLogs(_sut, LogLevel.Information, "Info");
        }

        /// <summary>
        /// Tests the log with level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void TestLogWithLevel(LogLevel logLevel)
        {
            _sut.Log(logLevel, logLevel.ToString());

            AssertLogs(_sut, logLevel, logLevel.ToString());
        }

        /// <summary>
        /// Tests the log with log event.
        /// </summary>
        [Fact]
        public void TestLogWithLogEvent()
        {
            var logEvent = new TestLogEvent(Category, LogLevel.Error, Message, true);

            _sut.Log(logEvent);

            var logs = _sut.GetSnapshot();
            logs.Count.Should().Be(1);

            logs[0].Should().Be(logEvent);
        }

        /// <summary>
        /// Tests the log empty line.
        /// </summary>
        [Fact]
        public void TestLogEmptyLine()
        {
            _sut.LogEmptyLine();

            AssertLogs(_sut, LogLevel.None, string.Empty);
        }

        /// <summary>
        /// Tests the clean up async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanUpAsync()
        {
            _sut.LogWarning("Warning");
            await _sut.CleanUpAsync();

            AssertEmpty(_sut);
        }

        /// <summary>
        /// Tests the scope.
        /// </summary>
        [Fact]
        public void TestScope()
        {
            var scope = _sut.Scope("mystate");
            _sut.LogError("Error");

            var scope2 = _sut.Scope("second");
            _sut.LogWarning("Warning");

            scope2.Dispose();
            _sut.LogInformation("Info");

            scope.Dispose();
            _sut.LogEmptyLine();

            var logs = _sut.GetSnapshot();
            logs.Count.Should().Be(4);

            logs[0].Scope.Should().Be(scope);
            logs[1].Scope.Should().Be(scope2);
            logs[2].Scope.Should().Be(scope);
            logs[3].Scope.Should().BeNull();
        }

        /// <summary>
        /// Asserts the logs.
        /// </summary>
        /// <param name="sut">The sut.</param>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        private static void AssertLogs(TestLogger sut, LogLevel level, string message)
        {
            var logs = sut.GetSnapshot();
            logs.Count.Should().Be(1);

            var log = logs[0];
            log.Category.Should().Be(TestLogger.Category);
            log.CreatedAt.Should().NotBe(default);
            log.IsExpected.Should().BeTrue();
            log.Level.Should().Be(level);
            log.Message.Should().Be(message);
            log.Scope.Should().BeNull();
        }

        /// <summary>
        /// Asserts the empty state.
        /// </summary>
        /// <param name="sut">The sut.</param>
        private static void AssertEmpty(TestLogger sut)
        {
            sut.GetSnapshot().Count.Should().Be(0);
        }
    }
}
