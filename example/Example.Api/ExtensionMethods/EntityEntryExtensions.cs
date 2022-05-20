using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq.Expressions;

namespace Example.Api.ExtensionMethods
{
    /// <summary>
    /// The entity entry extensions.
    /// </summary>
    public static class EntityEntryExtensions
    {
        /// <summary>
        /// Sets the property modified.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>An EntityEntry.</returns>
        public static EntityEntry<TEntity> SetPropertyModified<TEntity, TProperty>(this EntityEntry<TEntity> entry, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : class
        {
            entry.Property(propertyExpression).IsModified = true;
            return entry;
        }
    }
}
