using Example.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Example.Api.Auth
{
    /// <summary>
    /// The auth options.
    /// </summary>
    public class AuthOptions : AuthenticationSchemeOptions
    {
    }

    /// <summary>
    /// The auth handler.
    /// </summary>
    public class AuthHandler : AuthenticationHandler<AuthOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthHandler"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="clock">The clock.</param>
        public AuthHandler(
            IOptionsMonitor<AuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        /// <inheritdoc/>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
            {
                return AuthenticateResult.Fail("No auth header!");
            }

            var dbContext = Context.RequestServices.GetRequiredService<AppDbContext>();
            var authValue = authorization[0];
            var userId = await dbContext.Users
                .Where(x => x.AccessToken == authValue)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (userId == default)
            {
                return AuthenticateResult.Fail("Bad credentials!");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            var ticket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
