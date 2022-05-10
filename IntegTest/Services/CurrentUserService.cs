using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace IntegTest.Services
{
    public interface ICurrentUserService
    {
        public int UserId { get; }
    }

    public class CurrentUserService : ICurrentUserService
    {
        public int UserId { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var id = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(id) && int.TryParse(id, out var userId))
            {
                UserId = userId;
            }
        }
    }
}
