using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IntegTest.Data
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(AccessToken), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [MaxLength(50)]
        public string Password { get; set; } = null!;

        [MaxLength(100)]
        public string? AccessToken { get; set; }

        public ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
