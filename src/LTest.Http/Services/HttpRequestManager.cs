using LTest.Helpers;
using LTest.Http.Configuration;
using LTest.Http.Helpers;
using LTest.Http.Interfaces;
using LTest.Http.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LTest.Http.Services
{
    public class HttpRequestManager
    {
        private readonly ConcurrentDictionary<string, HttpRequestContext> _activeRequests = new();

        public async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestContext context, LTestFacade facade)
        {
            var label = context.Label!;
            context.Request.Headers.TryAddWithoutValidation(Constants.BulletProveRequestID, label);
            _activeRequests.TryAdd(label, context);

            await HookHelper.RunHooksAsync<IBeforeHttpRequestHook>(facade, x => x.BeforeHttpRequestAsync(context));

            var httpClient = facade.HttpClient;
            var httpConfiguration = facade.GetRequiredService<HttpConfiguration>();
            facade.Logger.LogInformation(LogHelper.CreateRequestLog(context.Request, httpClient, httpConfiguration));

            var result = await StopwatchHelper.MeasureAsync(() => httpClient.SendAsync(context.Request));
            var response = result.ResultObject;
            facade.DisposableCollertor.Add(response);

            await HookHelper.RunHooksAsync<IAfterHttpRequestHook>(facade, x => x.AfterHttpRequestAsync(context));

            context.Request.Dispose();

            facade.Logger.LogInformation(LogHelper.CreateResponseLog(response, result.ElapsedMilliseconds, httpConfiguration));

            return response;
        }

        public bool TryGetHttpRequestContext(string requestId, out HttpRequestContext? context)
        {
            return _activeRequests.TryGetValue(requestId, out context);
        }
    }
}
