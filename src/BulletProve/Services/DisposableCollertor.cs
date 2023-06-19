namespace BulletProve.Services
{
    /// <summary>
    /// The disposable collertor.
    /// </summary>
    public sealed class DisposableCollertor : IDisposable, IAsyncDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly List<IAsyncDisposable> _asyncDisposables = new();

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void Add(IAsyncDisposable disposable)
        {
            _asyncDisposables.Add(disposable);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.ForEach(x => x.Dispose());
            _asyncDisposables.ForEach(x => ((IDisposable)x).Dispose());
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            _disposables.ForEach(x => x.Dispose());
            await Task.WhenAll(_asyncDisposables.Select(x => x.DisposeAsync().AsTask()));
            GC.SuppressFinalize(this);
        }
    }
}
