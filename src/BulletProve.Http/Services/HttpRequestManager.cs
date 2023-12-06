using BulletProve.Exceptions;
using BulletProve.Helpers;
using BulletProve.Hooks;
using BulletProve.Http.Configuration;
using BulletProve.Http.Helpers;
using BulletProve.Http.Interfaces;
using BulletProve.Http.Models;
using BulletProve.ServerLog;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BulletProve.Http.Services
{
    /// <summary>
    /// The http request manager.
    /// </summary>
    public class HttpRequestManager : IServerLogHandler
    {
        private readonly ConcurrentDictionary<string, HttpRequestContext> _activeRequests = new();
        private readonly HookRunner _hookRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestManager"/> class.
        /// </summary>
        /// <param name="hookRunner">The hook runner.</param>
        public HttpRequestManager(HookRunner hookRunner)
        {
            _hookRunner = hookRunner;
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scope">The server scope.</param>
        public async Task<HttpResponseMessage> ExecuteRequestAsync(HttpRequestContext context, ServerScope scope)
        {
            var label = context.Label!;
            context.Request.Headers.TryAddWithoutValidation(Constants.BulletProveRequestID, label);
            if (!_activeRequests.TryAdd(label, context))
                throw new BulletProveException($"A request with label '{label}' has already been started");

            await _hookRunner.RunHooksAsync<IBeforeHttpRequestHook>(x => x.BeforeHttpRequestAsync(context));

            var httpClient = scope.HttpClient;
            var httpConfiguration = scope.GetRequiredService<HttpConfiguration>();
            scope.Logger.LogInformation(LogHelper.CreateRequestLog(context.Request, httpClient, httpConfiguration));

            var result = await StopwatchHelper.MeasureAsync(() => httpClient.SendAsync(context.Request));
            var response = result.ResultObject;

            _activeRequests.TryRemove(label, out var _);
            scope.DisposableCollector.Add(response);

            await _hookRunner.RunHooksAsync<IAfterHttpRequestHook>(x => x.AfterHttpRequestAsync(context));

            context.Request.Dispose();

            scope.Logger.LogInformation(LogHelper.CreateResponseLog(response, result.ElapsedMilliseconds, httpConfiguration));

            return response;
        }

        /// <inheritdoc/>
        public void HandleServerLog(ServerLogEvent serverLogEvent)
        {
            var requestId = serverLogEvent.Scope?.RequestId;

            if (!string.IsNullOrEmpty(requestId) && _activeRequests.TryGetValue(requestId, out var context))
            {
                context.Logs.AddServerLog(serverLogEvent);
            }
        }

        /// <inheritdoc/>
        public bool IsAllowed(ServerLogEvent logEvent)
        {
            var requestId = logEvent.Scope?.RequestId;

            if (!string.IsNullOrEmpty(requestId) && _activeRequests.TryGetValue(requestId, out var context))
            {
                return context.ServerLogInspector.IsAllowed(logEvent);
            }

            return false;
        }

        /// <summary>
        /// Tries the get http request context.
        /// </summary>
        /// <param name="requestId">The request id.</param>
        /// <param name="context">The context.</param>
        public bool TryGetHttpRequestContext(string requestId, out HttpRequestContext? context)
        {
            return _activeRequests.TryGetValue(requestId, out context);
        }
    }
}
