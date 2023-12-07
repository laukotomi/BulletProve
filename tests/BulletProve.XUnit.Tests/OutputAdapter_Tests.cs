using NSubstitute;
using Xunit.Abstractions;

namespace BulletProve.XUnit.Tests
{
    /// <summary>
    /// The output adapter tests.
    /// </summary>
    public class OutputAdapter_Tests
    {
        private readonly ITestOutputHelper _output;
        private readonly OutputAdapter _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputAdapter_Tests"/> class.
        /// </summary>
        public OutputAdapter_Tests()
        {
            _output = Substitute.For<ITestOutputHelper>();
            _sut = new OutputAdapter(_output);
        }

        /// <summary>
        /// Tests the write line.
        /// </summary>
        [Fact]
        public void TestWriteLine()
        {
            _sut.WriteLine("aaa");
            _output.Received(1).WriteLine("aaa");
        }
    }
}
