using Example.Api.Data;
using System.Linq;

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
        public void Seed()
        {
            if (!_dbContext.Users.Any(x => x.Username == "Admin"))
            {
                _dbContext.Users.Add(new User
                {
                    Username = "Admin",
                    Password = "Admin"
                });

                _dbContext.SaveChanges();
            }
        }
    }
}
