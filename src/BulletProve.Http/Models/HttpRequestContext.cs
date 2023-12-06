using BulletProve.ServerLog;
using BulletProve.Services;

namespace BulletProve.Http.Models
{
    /// <summary>
    /// The http request context.
    /// </summary>
    public sealed class HttpRequestContext : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestContext"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        public HttpRequestContext(string label)
        {
            Label = label;
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public HttpRequestMessage Request { get; } = new();

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the logs.
        /// </summary>
        public IServerLogCollector Logs { get; } = new ServerLogCollector();

        /// <summary>
        /// Gets the server log inspector.
        /// </summary>
        public Inspector<ServerLogEvent> ServerLogInspector { get; } = new();

        /// <inheritdoc />
        public void Dispose()
        {
            Request?.Dispose();
        }
    }
}
