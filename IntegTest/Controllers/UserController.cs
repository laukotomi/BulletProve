using AutoMapper;
using AutoMapper.QueryableExtensions;
using IntegTest.Data;
using IntegTest.Mapping;
using IntegTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntegTest.Controllers
{
    public class UserController : ApiController
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IExternalService _externalService;

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

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserDataAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var dto = await _dbContext.Users
                .Where(x => x.Id == userId)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            return dto;
        }

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

        public class RegisterUserCommand
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        public class UserDto : IMapFrom<User>
        {
            public int Id { get; set; }
            public string Username { get; set; } = null!;
        }
    }
}
