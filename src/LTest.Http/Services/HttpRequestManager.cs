using LTest.Helpers;
using LTest.Hooks;
using LTest.Http.Configuration;
using LTest.Http.Helpers;
using LTest.Http.Interfaces;
using LTest.Http.Models;
using LTest.LogSniffer;
using LTest.ServerLog;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace LTest.Http.Services
{
    public class HttpRequestManager : IServerLogInspector
    {
        private readonly ConcurrentDictionary<string, HttpRequestContext> _activeRequests = new();
        private readonly HookRunner _hookRunner;

        public HttpRequestManager(HookRunner hookRunner)
        {
            _hookRunner = hookRunner;
        }

        public async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestContext context, ServerScope facade)
        {
            var label = context.Label!;
            context.Request.Headers.TryAddWithoutValidation(Constants.BulletProveRequestID, label);
            _activeRequests.TryAdd(label, context);

            await _hookRunner.RunHooksAsync<IBeforeHttpRequestHook>(x => x.BeforeHttpRequestAsync(context));

            var httpClient = facade.HttpClient;
            var httpConfiguration = facade.GetRequiredService<HttpConfiguration>();
            facade.Logger.LogInformation(LogHelper.CreateRequestLog(context.Request, httpClient, httpConfiguration));

            var result = await StopwatchHelper.MeasureAsync(() => httpClient.SendAsync(context.Request));
            var response = result.ResultObject;

            _activeRequests.TryRemove(label, out var _);
            facade.DisposableCollertor.Add(response);

            await _hookRunner.RunHooksAsync<IAfterHttpRequestHook>(x => x.AfterHttpRequestAsync(context));

            context.Request.Dispose();

            facade.Logger.LogInformation(LogHelper.CreateResponseLog(response, result.ElapsedMilliseconds, httpConfiguration));

            return response;
        }

        public bool IsServerLogEventAllowed(ServerLogEvent logEvent)
        {
            var requestId = logEvent.Scope?.RequestId;

            if (!string.IsNullOrEmpty(requestId) && _activeRequests.TryGetValue(requestId, out var context))
            {
                context.Logs.AddLast(logEvent);
                return context.ServerLogInspector.IsAllowed(logEvent);
            }

            return false;
        }

        public bool TryGetHttpRequestContext(string requestId, out HttpRequestContext? context)
        {
            return _activeRequests.TryGetValue(requestId, out context);
        }
    }
}
