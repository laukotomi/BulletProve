using LTest.ServerLog;
using LTest.Services;

namespace LTest.Http.Models
{
    public sealed class HttpRequestContext : IDisposable
    {
        public HttpRequestContext(string label)
        {
            Label = label;
        }

        public HttpRequestMessage Request { get; } = new();
        public string Label { get; set; }
        public LinkedList<ServerLogEvent> Logs { get; } = new();
        public Inspector<ServerLogEvent> ServerLogInspector { get; } = new();

        public void Dispose()
        {
            Request?.Dispose();
        }
    }
}
