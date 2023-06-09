using LTest.Logging;
using LTest.Services;
using LTest.TestServer;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Log sniffer service implementation.
    /// </summary>
    public class DefaultServerLogInspector : ILogSnifferService, IServerLogInspector
    {
        private readonly LinkedList<ServerLogEvent> _serverLogs = new();
        private readonly object _lock = new();

        public Inspector<ServerLogEvent> ServerLogInspector { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultServerLogInspector"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public DefaultServerLogInspector(ServerConfigurator configurator)
        {
            ServerLogInspector = configurator.ServerLogInspector;
        }

        /// <summary>
        /// Returns the actual snapshot of the events.
        /// </summary>
        public IReadOnlyCollection<ServerLogEvent> GetServerLogs()
        {
            lock (_lock)
            {
                return _serverLogs.ToList();
            }
        }

        /// <summary>
        /// Resets the service.
        /// </summary>
        public Task ResetAsync()
        {
            lock (_lock)
            {
                _serverLogs.Clear();
                ServerLogInspector.Reset();
            }

            return Task.CompletedTask;
        }

        public bool IsServerLogEventAllowed(ServerLogEvent logEvent)
        {
            lock (_lock)
            {
                _serverLogs.AddLast(logEvent);
                return ServerLogInspector.IsAllowed(logEvent);
            }
        }
    }
}