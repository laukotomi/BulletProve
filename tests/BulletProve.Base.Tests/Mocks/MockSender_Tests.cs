using BulletProve.Mocks;
using FluentAssertions;

namespace BulletProve.Tests.Mocks
{
    /// <summary>
    /// The mock sender_ tests.
    /// </summary>
    public class MockSender_Tests : MockSender<string>
    {
        /// <summary>
        /// Tests the empty sender.
        /// </summary>
        [Fact]
        public void TestEmptySender()
        {
            Messages.Should().BeEmpty();
            SentMessages.Should().BeEmpty();
        }

        /// <summary>
        /// Tests the send message.
        /// </summary>
        [Fact]
        public void TestSendMessage()
        {
            Messages.Add("hello");
            SentMessages.Should().HaveCount(1).And.HaveElementAt(0, "hello");
        }

        /// <summary>
        /// Tests the clean up.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanUp()
        {
            Messages.Add("hello");
            await CleanUpAsync();

            Messages.Should().BeEmpty();
            SentMessages.Should().BeEmpty();
        }
    }
}
