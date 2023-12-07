using Microsoft.EntityFrameworkCore;

namespace BulletProve.EfCore.Services
{
    /// <summary>
    /// The database cleanup service.
    /// </summary>
    public interface IDatabaseCleanupService
    {
        /// <summary>
        /// Cleans up database.
        /// </summary>
        /// <param name="context">The DbContext.</param>
        Task CleanupAsync(DbContext context);
    }
}