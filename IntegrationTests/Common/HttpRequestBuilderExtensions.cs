using IntegTest.Controllers;
using LTest;
using LTest.Http.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;

namespace IntegrationTests.Common
{
    public static class HttpRequestBuilderExtensions
    {
        public static HttpRequestService CreateRequest<TController>(this HttpRequestBuilder builder, Func<TController, string> actionNameSelector, string token = null)
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

        public static HttpRequestService CreateRequest<TController>(this HttpRequestBuilder<TController> builder, Func<TController, string> actionNameSelector, string token = null)
            where TController : ControllerBase
        {
            return CreateRequest(builder as HttpRequestBuilder, actionNameSelector, token);
        }

        public static string LoginAsAdminAndGetToken(this HttpRequestBuilder builder, IntegrationTestServiceProvider services)
        {
            var authController = services.GetHttpRequestBuilder<AuthController>();

            var token = authController
                .CreateFor(x => nameof(x.LoginAsync))
                .SetJsonContent(new AuthController.LoginCommand
                {
                    Username = TestConstants.AdminUsername,
                    Password = TestConstants.AdminPassword
                })
                .Assert<string>()
                .EnsureSuccessStatusCode()
                .Execute();

            return token;
        }
    }
}
