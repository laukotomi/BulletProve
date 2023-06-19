using BulletProve.Hooks;
using BulletProve.Services;
using BulletProve.TestServer;

namespace BulletProve.ServerLog
{
    /// <summary>
    /// Log sniffer service implementation.
    /// </summary>
    public class ServerLogsService : IServerLogsService, IServerLogInspector, ICleanUpHook
    {
        private readonly LinkedList<ServerLogEvent> _serverLogs = new();
        private readonly object _lock = new();

        /// <inheritdoc />
        public Inspector<ServerLogEvent> ServerLogInspector { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerLogsService"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public ServerLogsService(ServerConfigurator configurator)
        {
            ServerLogInspector = configurator.ServerLogInspector;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<ServerLogEvent> GetServerLogs()
        {
            lock (_lock)
            {
                return _serverLogs.ToList();
            }
        }

        /// <inheritdoc />
        public bool IsServerLogEventAllowed(ServerLogEvent logEvent)
        {
            lock (_lock)
            {
                _serverLogs.AddLast(logEvent);
                return ServerLogInspector.IsAllowed(logEvent);
            }
        }

        /// <inheritdoc />
        public Task CleanUpAsync()
        {
            lock (_lock)
            {
                _serverLogs.Clear();
                ServerLogInspector.Reset();
            }

            return Task.CompletedTask;
        }
    }
}