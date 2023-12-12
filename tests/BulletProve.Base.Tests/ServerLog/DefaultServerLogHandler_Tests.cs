using BulletProve.Logging;
using BulletProve.ServerLog;
using BulletProve.TestServer;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BulletProve.Tests.ServerLog
{
    /// <summary>
    /// The default server log handler tests.
    /// </summary>
    public class DefaultServerLogHandler_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Category";

        private readonly ServerConfigurator _configurator;
        private readonly ITestLogger _logger;
        private readonly IServerLogCollector _serverLogCollector;
        private readonly DefaultServerLogHandler _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerLogHandler_Tests"/> class.
        /// </summary>
        public DefaultServerLogHandler_Tests()
        {
            _configurator = new ServerConfigurator();
            _logger = Substitute.For<ITestLogger>();
            _serverLogCollector = Substitute.For<IServerLogCollector>();
            _sut = new DefaultServerLogHandler(_configurator, _logger, _serverLogCollector);
        }

        /// <summary>
        /// Tests the is allowed.
        /// </summary>
        /// <param name="isAllowed">If true, is allowed.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestIsAllowed(bool isAllowed)
        {
            _configurator.ServerLogInspector.AddDefaultAllowedAction(x => isAllowed, string.Empty);

            var logEvent = new ServerLogEvent(Category, LogLevel.Warning, new EventId(), "Message", null, null);
            _sut.IsAllowed(logEvent).Should().Be(isAllowed);
        }

        /// <summary>
        /// Tests the handle server log minimum log level.
        /// </summary>
        [Fact]
        public void TestHandleServerLogMinimumLogLevel()
        {
            _configurator.LoggerCategoryNameInspector.AddDefaultAllowedAction(x => x == Category, string.Empty);
            _configurator.MinimumLogLevel = LogLevel.Warning;

            var logEvent = new ServerLogEvent(Category, LogLevel.Warning, new EventId(), "Message", null, null);
            _sut.HandleServerLog(logEvent);

            var logEvent2 = new ServerLogEvent(Category, LogLevel.Information, new EventId(), "Message", null, null);
            _sut.HandleServerLog(logEvent2);

            _logger.Received(1).Log(Arg.Any<TestLogEvent>());
        }

        /// <summary>
        /// Tests the handle server log category name.
        /// </summary>
        [Fact]
        public void TestHandleServerLogCategoryName()
        {
            _configurator.LoggerCategoryNameInspector.AddDefaultAllowedAction(x => x == Category, string.Empty);
            _configurator.MinimumLogLevel = LogLevel.Warning;

            var logEvent = new ServerLogEvent(Category, LogLevel.Warning, new EventId(), "Message", null, null);
            _sut.HandleServerLog(logEvent);

            var logEvent2 = new ServerLogEvent(Category + "A", LogLevel.Warning, new EventId(), "Message", null, null);
            _sut.HandleServerLog(logEvent2);

            _logger.Received(1).Log(Arg.Any<TestLogEvent>());
        }

        /// <summary>
        /// Tests the handle server log unexpected.
        /// </summary>
        [Fact]
        public void TestHandleServerLogUnexpected()
        {
            _configurator.LoggerCategoryNameInspector.AddDefaultAllowedAction(x => x == Category, string.Empty);
            _configurator.MinimumLogLevel = LogLevel.Warning;

            var logEvent = new ServerLogEvent(Category + "A", LogLevel.Information, new EventId(), "Message", null, null)
            {
                IsUnexpected = true
            };
            _sut.HandleServerLog(logEvent);

            _logger.Received(1).Log(Arg.Any<TestLogEvent>());
        }

        /// <summary>
        /// Tests the handle server log add server log.
        /// </summary>
        [Fact]
        public void TestHandleServerLogAddServerLog()
        {
            _configurator.LoggerCategoryNameInspector.AddDefaultAllowedAction(x => x == Category, string.Empty);
            _configurator.MinimumLogLevel = LogLevel.Warning;

            var logEvent = new ServerLogEvent(Category + "A", LogLevel.Information, new EventId(), "Message", null, null);
            _sut.HandleServerLog(logEvent);

            _serverLogCollector.Received(1).AddServerLog(logEvent);
        }
    }
}
