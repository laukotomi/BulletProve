using FluentAssertions;

namespace BulletProve.Http.Tests.Models
{
    /// <summary>
    /// The empty response tests.
    /// </summary>
    public class EmptyResponse_Tests
    {
        /// <summary>
        /// Tests the value.
        /// </summary>
        [Fact]
        public void TestValue()
        {
            EmptyResponse.Value.Should().NotBeNull();
        }
    }
}
