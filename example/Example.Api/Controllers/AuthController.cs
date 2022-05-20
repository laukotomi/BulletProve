using Example.Api.Data;
using Example.Api.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Api.Controllers
{
    /// <summary>
    /// The auth controller.
    /// </summary>
    public class AuthController : ApiController
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Login action.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string>> LoginAsync([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var userId = await _dbContext.Users
                .Where(x => x.Username == command.Username && x.Password == command.Password)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (userId == default)
            {
                return Unauthorized();
            }

            var token = Guid.NewGuid().ToString();

            _dbContext
                .StartUpdate(new User
                {
                    Id = userId,
                    AccessToken = token
                })
                .SetPropertyModified(x => x.AccessToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return token;
        }

        /// <summary>
        /// The login command.
        /// </summary>
        public class LoginCommand
        {
            /// <summary>
            /// Gets the username.
            /// </summary>
            public string? Username { get; init; }

            /// <summary>
            /// Gets the password.
            /// </summary>
            public string? Password { get; init; }
        }
    }
}
