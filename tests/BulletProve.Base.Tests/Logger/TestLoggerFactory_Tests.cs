using BulletProve.Logging;
using BulletProve.ServerLog;
using FluentAssertions;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The test logger factory tests.
    /// </summary>
    public class TestLoggerFactory_Tests
    {
        private readonly TestLoggerFactory _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestLoggerFactory_Tests"/> class.
        /// </summary>
        public TestLoggerFactory_Tests()
        {
            var provider = new ScopeProvider();
            _sut = new TestLoggerFactory(new List<IServerLogHandler>(), provider);
        }

        /// <summary>
        /// Tests the create logger.
        /// </summary>
        [Fact]
        public void TestCreateLogger()
        {
            var logger = _sut.CreateLogger("category");
            logger.GetType().Should().Be(typeof(LoggerProvider));
        }

        /// <summary>
        /// Tests the add provider.
        /// </summary>
        [Fact]
        public void TestAddProvider()
        {
            var act = () => _sut.AddProvider(null!);
            act.Should().Throw<NotImplementedException>();
        }

        /// <summary>
        /// Tests the dispose.
        /// </summary>
        [Fact]
        public void TestDispose()
        {
            var act = () => _sut.Dispose();
            act.Should().NotThrow<Exception>();
        }
    }
}
