using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTest.EFCore.Services
{
    /// <summary>
    /// Cleans up database.
    /// </summary>
    public class DatabaseCleanupService
    {
        private readonly TopologicalSortService _topologicalSortService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseCleanupService"/> class.
        /// </summary>
        /// <param name="topologicalSortService"></param>
        public DatabaseCleanupService(
            TopologicalSortService topologicalSortService)
        {
            _topologicalSortService = topologicalSortService;
        }

        /// <summary>
        /// Cleans up database.
        /// </summary>
        /// <param name="context">DbContext.</param>
        public async Task CleanupAsync(DbContext context)
        {
            var sql = GenerateCleaningSql(context);
            await context.Database.ExecuteSqlRawAsync(sql);
        }

        private string GenerateCleaningSql(DbContext context)
        {
            var entityTypes = context.Model.GetEntityTypes().ToList();

            var sorted = _topologicalSortService.Sort(entityTypes, entityType =>
                entityType.GetNavigations()
                .Where(x => x.ForeignKey.DeclaringEntityType == entityType)
                .Where(x => x.ForeignKey.PrincipalEntityType != entityType)
                .Where(x => x.ForeignKey.DeleteBehavior != DeleteBehavior.SetNull)
                .Select(x => x.ForeignKey.PrincipalEntityType));

            var sb = new StringBuilder();
            foreach (var entityType in sorted.Reverse())
            {
                var table = GetTableNameFromEntityType(entityType);
                var sql = $"DELETE FROM {table};";
                sb.AppendLine(sql);
            }

            return sb.ToString();
        }

        private object GetTableNameFromEntityType(IEntityType entityType)
        {
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (string.IsNullOrEmpty(schema))
            {
                return $"\"{tableName}\"";
            }

            return $"\"{schema}\".\"{tableName}\"";
        }
    }
}