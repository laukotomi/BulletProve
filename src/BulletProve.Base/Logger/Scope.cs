using System.Text.Json;

namespace BulletProve.Logging
{
    /// <summary>
    /// The log scope.
    /// </summary>
    public sealed class Scope : IDisposable
    {
        private readonly ScopeProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="state">The state.</param>
        /// <param name="parent">The parent.</param>
        internal Scope(ScopeProvider provider, object? state, string category, Scope? parent)
        {
            _provider = provider;
            State = state;
            Category = category;
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
                    IsHelperScope = true;
                    GroupId = RequestId;
                }
            }
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the group id.
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// Gets a value indicating whether scope is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is helper scope.
        /// </summary>
        public bool IsHelperScope { get; }

        /// <summary>
        /// Gets the level of the scope.
        /// </summary>
        public int Level { get; }

        /// <summary>
        /// Gets the parent scope.
        /// </summary>
        public Scope? Parent { get; }

        /// <summary>
        /// Gets the request id.
        /// </summary>
        public string? RequestId { get; }

        /// <summary>
        /// Gets the state object.
        /// </summary>
        public object? State { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                _provider.ScopeDisposeCallback(this);
            }
        }

        /// <summary>
        /// Tos the string.
        /// </summary>
        /// <returns>A string? .</returns>
        public override string? ToString()
        {
            return State is string ? State.ToString() : JsonSerializer.Serialize(State);
        }
    }
}
