namespace BulletProve.Services
{
    /// <summary>
    /// The disposable collertor.
    /// </summary>
    public sealed class DisposableCollector : IDisposable, IAsyncDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private readonly List<IAsyncDisposable> _asyncDisposables = new();

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void Add(IDisposable disposable)
        {
            if (disposable is IAsyncDisposable asyncDisposable)
            {
                _asyncDisposables.Add(asyncDisposable);
            }

            _disposables.Add(disposable);
        }

        /// <summary>
        /// Adds the.
        /// </summary>
        /// <param name="disposable">The disposable.</param>
        public void AddAsync(IAsyncDisposable disposable)
        {
            _asyncDisposables.Add(disposable);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.ForEach(x => x.Dispose());
            _asyncDisposables.ForEach(x =>
            {
                x.DisposeAsync().AsTask().Wait();
            });
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
