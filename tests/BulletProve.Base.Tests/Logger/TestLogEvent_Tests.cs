using BulletProve.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The test log event tests.
    /// </summary>
    public class TestLogEvent_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "category";

        /// <summary>
        /// The message.
        /// </summary>
        private const string Message = "message";

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestConstructor(bool isExpected)
        {
            var provider = new ScopeProvider();
            using var scope = new Scope(provider, "state", Category, null);

            var logEvent = new TestLogEvent(Category, LogLevel.Warning, Message, isExpected, scope);

            logEvent.Category.Should().Be(Category);
            logEvent.CreatedAt.Should().NotBe(default);
            logEvent.IsExpected.Should().Be(isExpected);
            logEvent.Level.Should().Be(LogLevel.Warning);
            logEvent.Message.Should().Be(Message);
            logEvent.Scope.Should().Be(scope);
        }

        /// <summary>
        /// Tests the to string expected.
        /// </summary>
        [Fact]
        public void TestToStringExpected()
        {
            var logEvent = new TestLogEvent(Category, LogLevel.Warning, Message, true, null);

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
            var logEvent = new TestLogEvent(Category, LogLevel.Error, Message, false, null);

            logEvent.Scope.Should().BeNull();
            var str = logEvent.ToString();

            str.Should().StartWith("EU:");
        }
    }
}
