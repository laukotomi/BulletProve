using BulletProve.Logging;
using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.ServerLog
{
    /// <summary>
    /// The server log event tests.
    /// </summary>
    public class ServerLogEvent_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Category";

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "Message";

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            using var scope = new Scope(new ScopeProvider(), "state", Category, null);
            var exception = new InvalidOperationException();
            var eventId = new EventId();

            var logEvent = new ServerLogEvent(Category, LogLevel.Error, eventId, Message, scope, exception);

            logEvent.CategoryName.Should().Be(Category);
            logEvent.EventId.Should().Be(eventId);
            logEvent.Exception.Should().Be(exception);
            logEvent.IsUnexpected.Should().BeFalse();
            logEvent.Level.Should().Be(LogLevel.Error);
            logEvent.Message.Should().Be(Message);
            logEvent.Scope.Should().Be(scope);

            logEvent.ToString().Should().Be("E: Message [Category] [0]");
        }

        /// <summary>
        /// Tests the to string expected.
        /// </summary>
        [Fact]
        public void TestToStringExpected()
        {
            var logEvent = new ServerLogEvent(Category, LogLevel.Warning, new EventId(), Message, null, null)
            {
                IsUnexpected = false
            };

            logEvent.Scope.Should().BeNull();
            var str = logEvent.ToString();

            str.Should().StartWith("W:");
            str.Should().Contain(Category);
            str.Should().Contain(Message);
        }

        /// <summary>
        /// Tests the to string unexpected.
        /// </summary>
        [Fact]
        public void TestToStringUnexpected()
        {
            var logEvent = new ServerLogEvent(Category, LogLevel.Error, new EventId(), Message, null, null)
            {
                IsUnexpected = true
            };

            logEvent.Scope.Should().BeNull();
            var str = logEvent.ToString();

            str.Should().StartWith("EU:");
        }
    }
}
