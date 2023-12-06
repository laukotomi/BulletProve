using BulletProve.Services;
using FluentAssertions;

namespace BulletProve.Tests.Services
{
    /// <summary>
    /// The inspector tests.
    /// </summary>
    public class Inspector_Tests
    {
        /// <summary>
        /// The label.
        /// </summary>
        private const string Label = "Label";

        private readonly Inspector<string> _sut;
        private readonly Func<string, bool> _action = x => x.Length == 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Inspector_Tests"/> class.
        /// </summary>
        public Inspector_Tests()
        {
            _sut = new Inspector<string>();
        }

        /// <summary>
        /// Tests the is allowed.
        /// </summary>
        [Fact]
        public void TestIsAllowed()
        {
            _sut.IsAllowed("aaa").Should().BeFalse();
        }

        /// <summary>
        /// Tests the add default allowed action.
        /// </summary>
        [Fact]
        public void TestAddDefaultAllowedAction()
        {
            _sut.AddDefaultAllowedAction(_action, Label);
            _sut.IsAllowed("aaa").Should().BeTrue();
        }

        /// <summary>
        /// Tests the add allowed action.
        /// </summary>
        [Fact]
        public void TestAddAllowedAction()
        {
            _sut.AddAllowedAction(_action, Label);
            _sut.IsAllowed("aaa").Should().BeTrue();
        }

        /// <summary>
        /// Tests the clear.
        /// </summary>
        [Fact]
        public void TestClear()
        {
            _sut.AddDefaultAllowedAction(_action, Label);
            _sut.AddAllowedAction(x => x.Length == 4, Label);

            _sut.Clear();

            _sut.IsAllowed("aaa").Should().BeFalse();
            _sut.IsAllowed("aaaa").Should().BeFalse();
        }

        /// <summary>
        /// Tests the reset.
        /// </summary>
        [Fact]
        public void TestReset()
        {
            _sut.AddDefaultAllowedAction(_action, Label);
            _sut.AddAllowedAction(x => x.Length == 4, Label);

            _sut.Reset();

            _sut.IsAllowed("aaa").Should().BeTrue();
            _sut.IsAllowed("aaaa").Should().BeFalse();
        }

        /// <summary>
        /// Tests the remove.
        /// </summary>
        [Fact]
        public void TestRemove()
        {
            _sut.AddDefaultAllowedAction(_action, Label);
            _sut.AddAllowedAction(x => x.Length == 4);

            _sut.Remove(Label);

            _sut.IsAllowed("aaa").Should().BeFalse();
            _sut.IsAllowed("aaaa").Should().BeTrue();
        }

        /// <summary>
        /// Tests the clean up async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanUpAsync()
        {
            _sut.AddDefaultAllowedAction(_action, Label);
            _sut.AddAllowedAction(x => x.Length == 4, Label);

            await _sut.CleanUpAsync();

            _sut.IsAllowed("aaa").Should().BeTrue();
            _sut.IsAllowed("aaaa").Should().BeFalse();
        }
    }
}
