using Microsoft.EntityFrameworkCore;

namespace BulletProve.EfCore.Services
{
    /// <summary>
    /// The sql executor.
    /// </summary>
    public interface ISqlExecutor
    {
        /// <summary>
        /// Executes the sql.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sql">The sql.</param>
        Task ExecuteAsync(DbContext context, string sql);
    }
}
