using BulletProve.Hooks;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// The server log collector.
    /// </summary>
    public class ServerLogCollector : IServerLogCollector, ICleanUpHook
    {
        private readonly LinkedList<ServerLogEvent> _serverLogs = new();
        private readonly object _lock = new();

        /// <inheritdoc/>
        public void AddServerLog(ServerLogEvent logEvent)
        {
            lock (_lock)
            {
                _serverLogs.AddLast(logEvent);
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<ServerLogEvent> GetServerLogs()
        {
            lock (_lock)
            {
                return _serverLogs.ToList();
            }
        }

        /// <inheritdoc />
        public Task CleanUpAsync()
        {
            lock (_lock)
            {
                _serverLogs.Clear();
            }

            return Task.CompletedTask;
        }
    }
}
