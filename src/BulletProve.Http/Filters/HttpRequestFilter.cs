using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BulletProve.Http.Filters
{
    /// <summary>
    /// The http request filter.
    /// </summary>
    public class HttpRequestFilter : IStartupFilter
    {
        /// <inheritdoc />
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
                            new(Constants.BulletProveRequestID, header)
                        });
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
