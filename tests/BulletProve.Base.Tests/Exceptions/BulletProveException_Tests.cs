using BulletProve.Exceptions;
using FluentAssertions;

namespace BulletProve.Base.Tests.Exceptions
{
    /// <summary>
    /// The bullet prove exception tests.
    /// </summary>
    public class BulletProveException_Tests
    {
        /// <summary>
        /// Tests the message constructor.
        /// </summary>
        [Fact]
        public void TestMessageConstructor()
        {
            var sut = new BulletProveException("message");
            sut.Message.Should().Be("message");
        }

        /// <summary>
        /// Tests the with inner exception.
        /// </summary>
        [Fact]
        public void TestWithInnerException()
        {
            var inner = new InvalidOperationException();
            var sut = new BulletProveException("message", inner);
            sut.Message.Should().Be("message");
            sut.InnerException.Should().Be(inner);
        }
    }
}
