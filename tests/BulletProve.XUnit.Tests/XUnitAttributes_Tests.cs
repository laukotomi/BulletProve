using FluentAssertions;

namespace BulletProve.XUnit.Tests
{
    /// <summary>
    /// The x unit attributes_ tests.
    /// </summary>
    public class XUnitAttributes_Tests
    {
        /// <summary>
        /// Tests the theory attribute.
        /// </summary>
        [Fact]
        public void TestTheoryAttribute()
        {
            var sut = new TheoryAttribute();
            sut.DisplayName.Should().Be(nameof(TestTheoryAttribute));
        }

        /// <summary>
        /// Tests the fact attribute.
        /// </summary>
        [Fact]
        public void TestFactAttribute()
        {
            var sut = new FactAttribute();
            sut.DisplayName.Should().Be(nameof(TestFactAttribute));
        }
    }
}
