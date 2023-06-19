using LTest.ServerLog;

namespace LTest.LogSniffer
{
    public interface IServerLogInspector
    {
        bool IsServerLogEventAllowed(ServerLogEvent logEvent);
    }
}
