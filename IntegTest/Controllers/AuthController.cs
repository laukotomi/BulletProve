using IntegTest.Data;
using IntegTest.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegTest.Controllers
{
    public class AuthController : ApiController
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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

        public class LoginCommand
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }
    }
}
