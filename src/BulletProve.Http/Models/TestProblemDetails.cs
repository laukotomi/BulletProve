using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace BulletProve.Http.Models
{
    /// <summary>
    /// The problem details. Basic implementation of Microsoft's ProblemDetails to deserialize.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TestProblemDetails
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public string? Type { get; init; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string? Title { get; init; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public HttpStatusCode Status { get; init; }

        /// <summary>
        /// Gets the trace id.
        /// </summary>
        public string? TraceId { get; init; }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public Dictionary<string, string[]> Errors { get; init; } = new();
    }
}
