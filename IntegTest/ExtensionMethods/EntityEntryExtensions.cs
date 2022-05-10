using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq.Expressions;

namespace IntegTest.ExtensionMethods
{
    public static class EntityEntryExtensions
    {
        public static EntityEntry<TEntity> SetPropertyModified<TEntity, TProperty>(this EntityEntry<TEntity> entry, Expression<Func<TEntity, TProperty>> propertyExpression)
            where TEntity : class
        {
            entry.Property(propertyExpression).IsModified = true;
            return entry;
        }
    }
}
