using LTest.Http;

namespace LTest.Logging
{
    public sealed class Scope : IDisposable
    {
        private readonly TestLogger _provider;

        public bool IsDisposed { get; private set; }

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

        public string GroupId { get; }

        public Scope? Parent { get; }

        public object? State { get; }

        public int Level { get; }

        public string? RequestId { get; }

        public override string? ToString()
        {
            return State?.ToString();
        }

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
