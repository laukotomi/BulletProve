using System.Collections.Generic;

namespace LTest.LogSniffer
{
    /// <summary>
    /// Category name collector.
    /// </summary>
    public class CategoryNameCollector
    {
        private readonly SortedSet<string> _categoryNames = new();
        private readonly object _lock = new();

        /// <summary>
        /// Adds a category name if was not added already.
        /// </summary>
        /// <param name="categoryName">Category name.</param>
        public void AddCategoryName(string categoryName)
        {
            lock(_lock)
            {
                if (!_categoryNames.Contains(categoryName))
                {
                    _categoryNames.Add(categoryName);
                }
            }
        }

        /// <summary>
        /// Returns collected category names.
        /// </summary>
        public IReadOnlySet<string> CategoryNames => _categoryNames;
    }
}