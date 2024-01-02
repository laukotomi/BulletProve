using BulletProve.Base.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Base.Tests.Configuration
{
    /// <summary>
    /// The logger configurator tests.
    /// </summary>
    public class LoggerConfigurator_Tests
    {
        private readonly LoggerConfigurator _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfigurator_Tests"/> class.
        /// </summary>
        public LoggerConfigurator_Tests()
        {
            _sut = new LoggerConfigurator();
        }

        /// <summary>
        /// Tests the configure server log inspector.
        /// </summary>
        [Fact]
        public void TestConfigureServerLogInspector()
        {
            _sut.ConfigureServerLogInspector(x => x.AddAllowedAction(le => le.CategoryName == "aaa"));

            var isAllowed = _sut.ServerLogInspector.IsAllowed(new ServerLog.ServerLogEvent("aaa", LogLevel.Information, new EventId(), string.Empty, null, null));
            isAllowed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the configure logger category name inspector.
        /// </summary>
        [Fact]
        public void TestConfigureLoggerCategoryNameInspector()
        {
            _sut.ConfigureAllowedLoggerCategoryNames(x => x.AddAllowedAction(cn => cn == "aaa"));

            var isAllowed = _sut.LoggerCategoryNameInspector.IsAllowed("aaa");
            isAllowed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the set minimum log level.
        /// </summary>
        [Fact]
        public void TestSetMinimumLogLevel()
        {
            _sut.SetMinimumLogLevel(LogLevel.Error);
            _sut.MinimumLogLevel.Should().Be(LogLevel.Error);
        }
    }
}
