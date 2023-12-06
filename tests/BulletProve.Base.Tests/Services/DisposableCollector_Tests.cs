using BulletProve.Services;
using FluentAssertions;

namespace BulletProve.Tests.Services
{
    /// <summary>
    /// The disposable collector tests.
    /// </summary>
    public class DisposableCollector_Tests
    {
        /// <summary>
        /// Tests the disposable.
        /// </summary>
        [Fact]
        public void TestDisposable()
        {
            var disposable = new Disposable();
            var asyncDisposable = new AsyncDisposable();
            var sut = new DisposableCollector();

            sut.Add(disposable);
            sut.AddAsync(asyncDisposable);
            sut.Dispose();

            disposable.IsDisposed.Should().BeTrue();
            asyncDisposable.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the async disposable.
        /// </summary>
        /// <returns>A Task.</returns>
        [Fact]
        public async Task TestAsyncDisposable()
        {
            var disposable = new Disposable();
            var asyncDisposable = new AsyncDisposable();
            var sut = new DisposableCollector();

            sut.Add(disposable);
            sut.AddAsync(asyncDisposable);
            await sut.DisposeAsync();

            disposable.IsDisposed.Should().BeTrue();
            asyncDisposable.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// Tests the add with async disposable.
        /// </summary>
        [Fact]
        public void TestAddWithAsyncDisposable()
        {
            var asyncDisposable = new AsyncDisposable();
            var sut = new DisposableCollector();

            sut.Add(asyncDisposable);
            sut.Dispose();

            asyncDisposable.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// The disposable.
        /// </summary>
        private sealed class Disposable : IDisposable
        {
            /// <summary>
            /// Gets a value indicating whether is disposed.
            /// </summary>
            public bool IsDisposed { get; private set; }

            /// <inheritdoc/>
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        /// <summary>
        /// The async disposable.
        /// </summary>
        private sealed class AsyncDisposable : IAsyncDisposable, IDisposable
        {
            /// <summary>
            /// Gets a value indicating whether is disposed.
            /// </summary>
            public bool IsDisposed { get; private set; }

            /// <inheritdoc/>
            public void Dispose()
            {
                IsDisposed = true;
            }

            /// <inheritdoc/>
            public ValueTask DisposeAsync()
            {
                IsDisposed = true;
                return ValueTask.CompletedTask;
            }
        }
    }
}
