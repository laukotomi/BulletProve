using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace BulletProve.EfCore.Services
{
    /// <summary>
    /// The sql executor.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class SqlExecutor : ISqlExecutor
    {
        /// <inheritdoc/>
        public Task ExecuteAsync(DbContext context, string sql)
        {
            return context.Database.ExecuteSqlRawAsync(sql);
        }
    }
}
