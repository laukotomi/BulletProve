using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Example.Api.Data
{
    /// <summary>
    /// The user.
    /// </summary>
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(AccessToken), IsUnique = true)]
    public class User
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [MaxLength(50)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        [MaxLength(100)]
        public string? AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the posts.
        /// </summary>
        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
