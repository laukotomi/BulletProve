using BulletProve.Http.Models;
using FluentAssertions;

namespace BulletProve.Http.Tests.Models
{
    /// <summary>
    /// The link generator context tests.
    /// </summary>
    public class LinkGeneratorContext_Tests
    {
        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            var sut = new LinkGeneratorContext(HttpMethod.Delete, "controller", "action");
            sut.ActionName.Should().Be("action");
            sut.ControllerName.Should().Be("controller");
            sut.Method.Should().Be(HttpMethod.Delete);
        }
    }
}
