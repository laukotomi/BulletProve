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
        public static Task<HttpResponseMessage> ExecuteRequestAsync(this HttpRequestBuilder builder, Action<IAssertionBuilder<HttpResponseMessage>>? assertionAction = null)
        {
            return builder.ExecuteRequestAsync(assertionAction);
        }

        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public static Task<HttpResponseMessage> ExecuteSuccessAsync(this HttpRequestBuilder builder, Action<IAssertionBuilder<HttpResponseMessage>>? assertionAction = null)
        {
            return builder.ExecuteRequestAsync(assert =>
            {
                assert.EnsureSuccessStatusCode();
                assertionAction?.Invoke(assert);
            });
        }

        /// <summary>
        /// Executes the request asserting success status code.
        /// </summary>
        /// <returns>An AssertBuilder.</returns>
        public static Task<TResponse> ExecuteSuccessAsync<TResponse>(this HttpRequestBuilder builder, Action<IAssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return builder.ExecuteRequestAsync<TResponse>(assert =>
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
        public static Task<HttpResponseMessage> ExecuteAssertingStatusAsync(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<HttpResponseMessage>>? assertionAction = null)
        {
            return builder.ExecuteRequestAsync(assert =>
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
        public static Task<TResponse> ExecuteAssertingStatusAsync<TResponse>(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<TResponse>>? assertionAction = null)
            where TResponse : class
        {
            return builder.ExecuteRequestAsync<TResponse>(assert =>
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
        public static Task<TestProblemDetails> ExecuteAssertingProblemAndStatusAsync(this HttpRequestBuilder builder, HttpStatusCode statusCode, Action<IAssertionBuilder<TestProblemDetails>>? assertionAction = null)
        {
            return ExecuteAssertingStatusAsync(builder, statusCode, assertionAction);
        }
    }
}
