using IntegTest.Data;
using System.Linq;

namespace IntegTest.Services
{
    public class Seeder
    {
        private readonly AppDbContext _dbContext;

        public Seeder(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
