using BulletProve.Services;
using FluentAssertions;

namespace BulletProve.Tests.Services
{
    /// <summary>
    /// The allowed action tests.
    /// </summary>
    public class AllowedAction_Tests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        /// <param name="isDefault">If true, is default.</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestConstructor(bool isDefault)
        {
            Func<string, bool> action = x => x.Length == 3;
            var sut = new AllowedAction<string>("ActionName", action, isDefault);
            sut.Action.Should().Be(action);
            sut.ActionName.Should().Be("ActionName");
            sut.IsDefault.Should().Be(isDefault);
        }
    }
}
