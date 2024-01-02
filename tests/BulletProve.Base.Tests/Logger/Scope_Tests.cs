using BulletProve.Logging;
using FluentAssertions;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The scope tests.
    /// </summary>
    public class Scope_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "Category";

        private readonly List<KeyValuePair<string, object>> _state =
        [
           new(Constants.BulletProveRequestID, "Value")
        ];
        private readonly ScopeProvider _provider;
        private static readonly int[] state = [3];

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope_Tests"/> class.
        /// </summary>
        public Scope_Tests()
        {
            _provider = new ScopeProvider();
        }

        /// <summary>
        /// Tests the scope.
        /// </summary>
        [Fact]
        public void TestScope()
        {
            var scope = new Scope(_provider, _state, Category, null);

            scope.Category.Should().Be(Category);
            scope.GroupId.Should().NotBeNullOrEmpty();
            scope.IsDisposed.Should().BeFalse();
            scope.IsHelperScope.Should().BeTrue();
            scope.Level.Should().Be(0);
            scope.Parent.Should().BeNull();
            scope.State.Should().Be(_state);
            scope.RequestId.Should().Be("Value");

            scope.Dispose();
            scope.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the scope with parent.
        /// </summary>
        [Fact]
        public void TestScopeWithParent()
        {
            using var parent = new Scope(_provider, _state, Category, null);
            using var child = new Scope(_provider, "state", Category, parent);

            child.Category.Should().Be(Category);
            child.GroupId.Should().Be(parent.GroupId);
            child.IsDisposed.Should().BeFalse();
            child.IsHelperScope.Should().BeFalse();
            child.Level.Should().Be(1);
            child.Parent.Should().Be(parent);
            child.State.Should().Be("state");
            child.RequestId.Should().Be(parent.RequestId);
        }

        /// <summary>
        /// Tests the scope with string state.
        /// </summary>
        [Fact]
        public void TestScopeWithStringState()
        {
            using var scope = new Scope(_provider, "state", Category, null);

            scope.GroupId.Should().Be("state");
            scope.RequestId.Should().BeNull();
        }

        /// <summary>
        /// Tests the scope without bullet prove request i d.
        /// </summary>
        [Fact]
        public void TestScopeWithoutBulletProveRequestID()
        {
            using var scope = new Scope(_provider, new List<KeyValuePair<string, object>>(), Category, null);
            scope.RequestId.Should().BeNull();
        }

        /// <summary>
        /// Tests the to string with string.
        /// </summary>
        [Fact]
        public void TestToStringWithString()
        {
            using var scope = new Scope(_provider, "state", Category, null);
            scope.ToString().Should().Be("state");
        }

        /// <summary>
        /// Tests the to string with object.
        /// </summary>
        [Fact]
        public void TestToStringWithObject()
        {
            using var scope = new Scope(_provider, state, Category, null);
            scope.ToString().Should().Be("[3]");
        }
    }
}
