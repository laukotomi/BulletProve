using BulletProve.Http.Models;
using BulletProve.Http.Services;
using System.Net;

namespace BulletProve
{
    /// <summary>
    /// The http request builder extensions.
    /// </summary>
    public static class HttpRequestBuilderExtensions
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="assertionAction">The assertion action.</param>
        /// <returns>A Task.</returns>
        public static Task<HttpResponseMessage> ShouldExecuteAsync(this HttpRequestBuilder builder, Action<IAssertionBuilder<HttpResponseMessage>> assertionAction)
        {
            return builder.ShouldExecuteAsync(assertionAction);
        }

        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public static Task<HttpResponseMessage> ShouldExecuteSuccessfullyAsync(this HttpRequestBuilder builder, Action<IAssertionBuilder<HttpResponseMessage>>? assertionAction = null)
        {
            return builder.ShouldExecuteAsync(assert =>
            {
                assert.EnsureSuccessStatusCode();
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public static Task<TResponse> ShouldExecuteSuccessfullyAsync<TResponse>(this HttpRequestBuilder builder, Action<IAssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return builder.ShouldExecuteAsync<TResponse>(assert =>
            {
                assert.EnsureSuccessStatusCode();
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Executes the request asserting specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public static Task<HttpResponseMessage> ShouldExecuteWithStatusAsync(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<HttpResponseMessage>>? assertionAction = null)
        {
            return builder.ShouldExecuteAsync(assert =>
            {
                assert.AssertStatusCode(statusCode);
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Executes the request asserting specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public static Task<TResponse> ShouldExecuteWithStatusAsync<TResponse>(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return builder.ShouldExecuteAsync<TResponse>(assert =>
            {
                assert.AssertStatusCode(statusCode);
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Starts asserting problem details with specified status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns>An AssertBuilder.</returns>
        public static Task<TestProblemDetails> ShouldExecuteWithProblemAndStatusAsync(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<TestProblemDetails>>? assertionAction = null)
        {
            return ShouldExecuteWithStatusAsync(builder, statusCode, assertionAction);
        }
    }
}
