using LTest.Logging;
using System.Collections.Concurrent;

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
        public ConcurrentQueue<LTestLogEvent> Logs { get; } = new();

        public void Dispose()
        {
            Request?.Dispose();
        }
    }
}
