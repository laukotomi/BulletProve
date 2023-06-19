namespace BulletProve.ServerLog
{
    /// <summary>
    /// The server log inspector.
    /// </summary>
    public interface IServerLogInspector
    {
        /// <summary>
        /// Is the server log event allowed.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        bool IsServerLogEventAllowed(ServerLogEvent logEvent);
    }
}
