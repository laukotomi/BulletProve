﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace BulletProve.EfCore.Services
{
    /// <summary>
    /// Cleans up database.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="DatabaseCleanupService"/> class.
    /// </remarks>
    /// <param name="sqlExecutor">The sql executor.</param>
    public class DatabaseCleanupService(ISqlExecutor sqlExecutor) : IDatabaseCleanupService
    {
        private readonly TopologicalSortService _topologicalSortService = new();

        /// <inheritdoc/>
        public Task CleanupAsync(DbContext context)
        {
            var sql = GenerateCleaningSql(context);
            return sqlExecutor.ExecuteAsync(context, sql);
        }

        /// <summary>
        /// Generates the cleaning sql.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A string.</returns>
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

        /// <summary>
        /// Gets the table name from entity type.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        private static string GetTableNameFromEntityType(IEntityType entityType)
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