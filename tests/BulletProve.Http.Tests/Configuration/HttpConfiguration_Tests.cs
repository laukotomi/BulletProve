using BulletProve.Http.Configuration;
using BulletProve.Http.Services;
using FluentAssertions;

namespace BulletProve.Http.Tests.Configuration
{
    /// <summary>
    /// The http configuration tests.
    /// </summary>
    public class HttpConfiguration_Tests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var sut = new HttpConfiguration();
            sut.HttpContentLogMaxLength.Should().Be(1000);
            sut.HttpHeaderLogMaxLength.Should().Be(50);
            sut.ResponseMessageDeserializer.Should().BeOfType<JsonResponseMessageDeserializer>();
        }
    }
}
