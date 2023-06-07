using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LTest.Http.Filters
{
    public class HttpRequestFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.Use((ctx, nxt) =>
                {
                    IDisposable? scope = null;

                    if (ctx.Request.Headers.Remove(Constants.BulletProveRequestID, out var header))
                    {
                        var logger = ctx.RequestServices.GetRequiredService<ILogger<HttpRequestFilter>>();
                        scope = logger.BeginScope(new List<KeyValuePair<string, object>>
                        {
                            new KeyValuePair<string, object>(Constants.BulletProveRequestID, header)
                        }.AsReadOnly());
                    }

                    var result = nxt(ctx);
                    scope?.Dispose();
                    return result;
                });

                next(builder);
            };
        }
    }
}
