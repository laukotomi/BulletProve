using BulletProve.ServerLog;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.ServerLog
{
    /// <summary>
    /// The server log collector tests.
    /// </summary>
    public class ServerLogCollector_Tests
    {
        private readonly ServerLogCollector _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLogCollector_Tests"/> class.
        /// </summary>
        public ServerLogCollector_Tests()
        {
            _sut = new ServerLogCollector();
        }

        /// <summary>
        /// Tests the empty.
        /// </summary>
        [Fact]
        public void TestEmpty()
        {
            _sut.GetServerLogs().Should().HaveCount(0);
        }

        /// <summary>
        /// Tests the add server log.
        /// </summary>
        [Fact]
        public void TestAddServerLog()
        {
            var logEvent = new ServerLogEvent("cat", LogLevel.Information, new EventId(), "Message", null, null);
            var logEvent2 = new ServerLogEvent("cat", LogLevel.Information, new EventId(), "Message", null, null);

            _sut.AddServerLog(logEvent);
            _sut.AddServerLog(logEvent2);

            var logs = _sut.GetServerLogs();
            logs.Should().HaveCount(2);
            logs[0].Should().Be(logEvent);
            logs[1].Should().Be(logEvent2);
        }

        /// <summary>
        /// Tests the clean up async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanUpAsync()
        {
            var logEvent = new ServerLogEvent("cat", LogLevel.Information, new EventId(), "Message", null, null);
            _sut.AddServerLog(logEvent);

            await _sut.CleanUpAsync();
            _sut.GetServerLogs().Should().HaveCount(0);
        }
    }
}
