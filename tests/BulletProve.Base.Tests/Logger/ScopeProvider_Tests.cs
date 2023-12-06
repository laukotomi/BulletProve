using BulletProve.Logging;
using FluentAssertions;

namespace BulletProve.Tests.Logger
{
    /// <summary>
    /// The scope provider tests.
    /// </summary>
    public class ScopeProvider_Tests
    {
        /// <summary>
        /// The category.
        /// </summary>
        private const string Category = "category";

        private readonly ScopeProvider _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeProvider_Tests"/> class.
        /// </summary>
        public ScopeProvider_Tests()
        {
            _sut = new ScopeProvider();
        }

        /// <summary>
        /// Tests the constructor.
        /// </summary>
        [Fact]
        public void TestConstructor()
        {
            _sut.CurrentScope.Should().BeNull();
        }

        /// <summary>
        /// Tests the create scope.
        /// </summary>
        [Fact]
        public void TestCreateScope()
        {
            using var scope = _sut.CreateScope(Category);

            scope.Should().NotBeNull();
            _sut.CurrentScope.Should().Be(scope);
        }

        /// <summary>
        /// Tests the create scope more scopes.
        /// </summary>
        [Fact]
        public void TestCreateScopeMoreScopes()
        {
            var scope = _sut.CreateScope("mystate");

            _sut.CurrentScope.Should().Be(scope);

            var scope2 = _sut.CreateScope("second");

            _sut.GetOpenScopes().Should().HaveCount(2);
            _sut.CurrentScope.Should().Be(scope2);

            scope2.Dispose();
            _sut.CurrentScope.Should().Be(scope);

            scope.Dispose();
            _sut.CurrentScope.Should().BeNull();
            _sut.GetOpenScopes().Should().HaveCount(0);
        }

        /// <summary>
        /// Tests the scope dispose callback.
        /// </summary>
        [Fact]
        public void TestScopeDisposeCallback()
        {
            var scope = _sut.CreateScope(Category);
            scope.Dispose();

            _sut.CurrentScope.Should().BeNull();
        }

        /// <summary>
        /// Tests the scope dispose callback scope not disposed.
        /// </summary>
        [Fact]
        public void TestScopeDisposeCallbackScopeNotDisposed()
        {
            var scope = (Scope)_sut.CreateScope(Category);

            var act = () => _sut.ScopeDisposeCallback(scope);
            act.Should().Throw<InvalidOperationException>();

            scope.Dispose();
        }

        /// <summary>
        /// Tests the scope dispose callback wrong order.
        /// </summary>
        [Fact]
        public void TestScopeDisposeCallbackWrongOrder()
        {
            var scope1 = (Scope)_sut.CreateScope(Category);
            var scope2 = (Scope)_sut.CreateScope(Category);

            var act = scope1.Dispose;
            act.Should().Throw<InvalidOperationException>();

            scope2.Dispose();
            scope1.Dispose();
        }

        /// <summary>
        /// Tests the get open scopes.
        /// </summary>
        [Fact]
        public void TestGetOpenScopes()
        {
            var scope = (Scope)_sut.CreateScope(Category);

            _sut.GetOpenScopes().Should().HaveCount(1);

            scope.Dispose();

            _sut.GetOpenScopes().Should().HaveCount(0);
        }

        /// <summary>
        /// Tests the clean up async.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestCleanUpAsync()
        {
            await _sut.CleanUpAsync();
            _sut.CurrentScope.Should().BeNull();
        }
    }
}
