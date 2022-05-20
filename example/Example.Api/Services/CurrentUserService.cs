using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Example.Api.Services
{
    /// <summary>
    /// The current user service.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the user id.
        /// </summary>
        public int UserId { get; }
    }

    /// <summary>
    /// The current user service.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        /// <summary>
        /// Gets the user id.
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentUserService"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
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
