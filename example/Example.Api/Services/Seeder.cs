using Example.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Example.Api.Services
{
    /// <summary>
    /// The seeder.
    /// </summary>
    public class Seeder
    {
        /// <summary>
        /// The admin name.
        /// </summary>
        private const string _adminName = "Admin";

        private readonly AppDbContext _dbContext;
        private readonly string _adminPassword;

        /// <summary>
        /// Initializes a new instance of the <see cref="Seeder"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        public Seeder(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _adminPassword = configuration.GetValue<string>("AdminPassword");
        }

        /// <summary>
        /// Seeds the DB.
        /// </summary>
        public async Task SeedAsync()
        {
            var hasAdminUser = await _dbContext.Users.AnyAsync(x => x.Username == _adminName);
            if (!hasAdminUser)
            {
                _dbContext.Users.Add(new User
                {
                    Username = _adminName,
                    Password = _adminPassword
                });

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
