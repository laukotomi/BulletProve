using BulletProve.Http.Services;
using FluentAssertions;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The label generator service tests.
    /// </summary>
    public class LabelGeneratorService_Tests
    {
        /// <summary>
        /// Tests the get label.
        /// </summary>
        [Fact]
        public void TestGetLabel()
        {
            var sut = new LabelGeneratorService();
            var label1 = sut.GetLabel();
            var label2 = sut.GetLabel();

            label1.Should().Be("Request #1");
            label2.Should().Be("Request #2");
        }
    }
}
