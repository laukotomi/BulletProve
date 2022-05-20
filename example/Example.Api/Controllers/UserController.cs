using AutoMapper;
using AutoMapper.QueryableExtensions;
using Example.Api.Data;
using Example.Api.Mapping;
using Example.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Example.Api.Controllers
{
    /// <summary>
    /// The user controller.
    /// </summary>
    public class UserController : ApiController
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IExternalService _externalService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="currentUserService">The current user service.</param>
        /// <param name="dbContext">The db context.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="externalService">The external service.</param>
        public UserController(
            ICurrentUserService currentUserService,
            AppDbContext dbContext,
            IMapper mapper,
            IExternalService externalService)
        {
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _mapper = mapper;
            _externalService = externalService;
        }

        /// <summary>
        /// Gets the user data async.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        [HttpGet]
        public async Task<ActionResult<UserDto?>> GetUserDataAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var dto = await _dbContext.Users
                .Where(x => x.Id == userId)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return dto;
        }

        /// <summary>
        /// Registers the user async.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        [HttpPost]
        public async Task<IActionResult> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.UserName) || string.IsNullOrEmpty(command.Password))
            {
                return BadRequest();
            }

            var correct = await _externalService.IsUserCorrectAsync(command.UserName, cancellationToken);
            if (!correct)
            {
                return BadRequest();
            }

            _dbContext.Users.Add(new User
            {
                Username = command.UserName,
                Password = command.Password
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// The register user command.
        /// </summary>
        public class RegisterUserCommand
        {
            /// <summary>
            /// Gets the user name.
            /// </summary>
            public string? UserName { get; init; }

            /// <summary>
            /// Gets the password.
            /// </summary>
            public string? Password { get; init; }
        }

        /// <summary>
        /// The user dto.
        /// </summary>
        public class UserDto : IMapFrom<User>
        {
            /// <summary>
            /// Gets the id.
            /// </summary>
            public int Id { get; init; }

            /// <summary>
            /// Gets the username.
            /// </summary>
            public string Username { get; init; } = null!;
        }
    }
}
