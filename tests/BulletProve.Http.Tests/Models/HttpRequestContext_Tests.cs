using BulletProve.Http.Models;
using BulletProve.ServerLog;
using FluentAssertions;

namespace BulletProve.Http.Tests.Models
{
    /// <summary>
    /// The http request context_ tests.
    /// </summary>
    public class HttpRequestContext_Tests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            using var sut = new HttpRequestContext("label");
            sut.Label.Should().Be("label");
            sut.Logs.Should().NotBeNull().And.BeOfType<ServerLogCollector>();
            sut.ServerLogInspector.Should().NotBeNull();
            sut.Request.Should().NotBeNull();
        }

        /// <summary>
        /// Tests the dispose.
        /// </summary>
        [Fact]
        public void TestDispose()
        {
            var sut = new HttpRequestContext("label");
            var act = () => sut.Dispose();
            act.Should().NotThrow<Exception>();
        }
    }
}
