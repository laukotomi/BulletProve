using Example.Api.Controllers;
using Example.Api.IntegrationTests;
using LTest.Http.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IntegrationTests.Common
{
    /// <summary>
    /// The http request builder extensions.
    /// </summary>
    public static class HttpRequestBuilderExtensions
    {
        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="actionNameSelector">The action name selector.</param>
        /// <param name="token">The token.</param>
        /// <returns>A HttpRequestService.</returns>
        public static HttpRequestService CreateRequest<TController>(this HttpRequestBuilder builder, Expression<Func<TController, Delegate>> actionNameSelector, string? token = null)
            where TController : ControllerBase
        {
            var request = builder.CreateFor(actionNameSelector);

            if (!string.IsNullOrEmpty(token))
            {
                request.SetHeaders(x =>
                {
                    x.Authorization = new AuthenticationHeaderValue(token);
                });
            }

            return request;
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="actionNameSelector">The action name selector.</param>
        /// <param name="token">The token.</param>
        /// <returns>A HttpRequestService.</returns>
        public static HttpRequestService CreateRequest<TController>(this HttpRequestBuilder<TController> builder, Expression<Func<TController, Delegate>> actionNameSelector, string? token = null)
            where TController : ControllerBase
        {
            return CreateRequest(builder as HttpRequestBuilder, actionNameSelector, token);
        }

        /// <summary>
        /// Logins the as admin and get token.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns>A string.</returns>
        public static async Task<string> LoginAsAdminAndGetTokenAsync(this HttpRequestBuilder builder)
        {
            var token = await builder
                .CreateFor<AuthController>(x => x.LoginAsync)
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .ExecuteAsync();

            return token;
        }
    }
}
