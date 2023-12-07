using BulletProve.EfCore.Services;
using FluentAssertions;

namespace BulletProve.EfCore.Tests.Services
{
    /// <summary>
    /// The topological sort service tests.
    /// </summary>
    public class TopologicalSortService_Tests
    {
        private readonly TopologicalSortService _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopologicalSortService_Tests"/> class.
        /// </summary>
        public TopologicalSortService_Tests()
        {
            _sut = new TopologicalSortService();
        }

        /// <summary>
        /// Tests the sort.
        /// </summary>
        [Fact]
        public void TestSort()
        {
            int[] collection = [2, 3, 4];
            var sorted = _sut.Sort(collection, x =>
            {
                if (x == 2)
                    return [3, 4];
                else if (x == 3)
                    return [4];
                else
                    return [];
            });

            sorted[0].Should().Be(4);
            sorted[1].Should().Be(3);
            sorted[2].Should().Be(2);
        }

        /// <summary>
        /// Tests the cyclic.
        /// </summary>
        [Fact]
        public void TestCyclic()
        {
            int[] collection = [2, 3];
            var act = () => _sut.Sort(collection, x =>
            {
                if (x == 2)
                    return [3];
                else if (x == 3)
                    return [2];
                else
                    return [];
            });

            act.Should().Throw<ArgumentException>();
        }
    }
}
