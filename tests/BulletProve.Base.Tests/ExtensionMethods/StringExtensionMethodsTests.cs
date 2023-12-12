using BulletProve.ExtensionMethods;
using FluentAssertions;

namespace BulletProve.Tests.ExtensionMethods
{
    /// <summary>
    /// The string extension methods tests.
    /// </summary>
    public class StringExtensionMethodsTests
    {
        /// <summary>
        /// Tests the TrimEnd method.
        /// </summary>
        [Fact]
        public void TestTrimEnd()
        {
            var str = "something";
            var result = str.TrimEnd("thing");
            result.Should().Be("some");
        }

        /// <summary>
        /// Tests the trim end empty suffix.
        /// </summary>
        [Fact]
        public void TestTrimEndEmptySuffix()
        {
            var str = "str";
            var result = str.TrimEnd(string.Empty);
            result.Should().Be("str");
        }

        /// <summary>
        /// Tests the trim end no trim.
        /// </summary>
        [Fact]
        public void TestTrimEndNoTrim()
        {
            var str = "str";
            var result = str.TrimEnd("aaa");
            result.Should().Be("str");
        }

        /// <summary>
        /// Tests the Truncate method.
        /// </summary>
        [Fact]
        public void TestTruncate()
        {
            var str = "something";
            var result = str.Truncate(4);
            result.Should().Be("some...");
        }

        [Fact]
        public void TestTruncateNoTruncate()
        {
            var str = "some";
            var result = str.Truncate(4);
            result.Should().Be("some");
        }
    }
}
