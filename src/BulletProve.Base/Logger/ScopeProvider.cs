using BulletProve.Hooks;
using System.Collections.Concurrent;

namespace BulletProve.Logging
{
    /// <summary>
    /// The scope provider.
    /// </summary>
    public class ScopeProvider : ICleanUpHook
    {
        private readonly AsyncLocal<Scope?> _currentScope = new();
        private readonly ConcurrentBag<Scope> _scopes = new();

        /// <summary>
        /// Gets the current scope.
        /// </summary>
        public Scope? CurrentScope => _currentScope.Value;

        /// <summary>
        /// Creates a scope.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>An IDisposable.</returns>
        public IDisposable CreateScope(string category, object? state = null)
        {
            var newScope = new Scope(this, state, category, CurrentScope);
            _scopes.Add(newScope);
            _currentScope.Value = newScope;

            return newScope;
        }

        /// <summary>
        /// Disposes the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public void ScopeDisposeCallback(Scope scope)
        {
            if (!scope.IsDisposed)
                throw new InvalidOperationException("Scope is not disposed");

            if (!_scopes.IsEmpty && CurrentScope != scope)
                throw new InvalidOperationException($"Scope '{scope}' is disposed before '{CurrentScope}'");

            _currentScope.Value = scope.Parent;
        }

        /// <summary>
        /// Gets the open scopes.
        /// </summary>
        /// <returns>A read only collection of Scopes.</returns>
        public IReadOnlyCollection<Scope> GetOpenScopes()
        {
            return _scopes.Where(x => !x.IsDisposed).ToList();
        }

        /// <inheritdoc />
        public Task CleanUpAsync()
        {
            _currentScope.Value = null;
            _scopes.Clear();

            return Task.CompletedTask;
        }
    }
}
