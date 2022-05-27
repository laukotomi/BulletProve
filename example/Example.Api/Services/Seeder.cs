using Example.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Example.Api.Services
{
    /// <summary>
    /// The seeder.
    /// </summary>
    public class Seeder
    {
        private readonly AppDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Seeder"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        public Seeder(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Seeds the DB.
        /// </summary>
        public async Task SeedAsync()
        {
            var hasAdminUser = await _dbContext.Users.AnyAsync(x => x.Username == "Admin");
            if (!hasAdminUser)
            {
                _dbContext.Users.Add(new User
                {
                    Username = "Admin",
                    Password = "Admin"
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
