namespace BulletProve.ServerLog
{
    /// <summary>
    /// The server log collector.
    /// </summary>
    public interface IServerLogCollector
    {
        /// <summary>
        /// Adds the server log.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        void AddServerLog(ServerLogEvent logEvent);

        /// <summary>
        /// Gets the server logs.
        /// </summary>
        /// <returns>A list of ServerLogEvents.</returns>
        IReadOnlyList<ServerLogEvent> GetServerLogs();
    }
}
