using LTest.Logging;

namespace LTest.LogSniffer
{
    public interface IServerLogInspector
    {
        bool IsServerLogEventAllowed(ServerLogEvent logEvent);
    }
}
