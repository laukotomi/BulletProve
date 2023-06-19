namespace BulletProve.Logging
{
    /// <summary>
    /// The log scope.
    /// </summary>
    public sealed class Scope : IDisposable
    {
        private readonly TestLogger _provider;

        /// <summary>
        /// Gets a value indicating whether scope is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="state">The state.</param>
        /// <param name="parent">The parent.</param>
        internal Scope(TestLogger provider, object? state, Scope? parent)
        {
            _provider = provider;
            State = state;
            Parent = parent;

            if (parent != null)
            {
                Level = parent.Level + 1;
                RequestId = parent.RequestId;
                GroupId = parent.GroupId;
            }
            else
            {
                if (state is string str)
                {
                    GroupId = str;
                }
                else
                {
                    GroupId = Guid.NewGuid().ToString();
                }
            }

            if (string.IsNullOrEmpty(RequestId) && state is IReadOnlyList<KeyValuePair<string, object>> list)
            {
                RequestId = list.FirstOrDefault(x => x.Key == Constants.BulletProveRequestID).Value?.ToString();

                if (!string.IsNullOrEmpty(RequestId))
                {
                    GroupId = RequestId;
                }
            }
        }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// Gets the parent scope.
        /// </summary>
        public Scope? Parent { get; }

        /// <summary>
        /// Gets the state object.
        /// </summary>
        public object? State { get; }

        /// <summary>
        /// Gets the level of the scope.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the request id.
        /// </summary>
        public string? RequestId { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!IsDisposed)
            {
                _provider.CurrentScope.Value = Parent;
                IsDisposed = true;
            }
        }
    }
}
