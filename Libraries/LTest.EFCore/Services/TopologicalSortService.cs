using System;
using System.Collections.Generic;
using System.Linq;

namespace LTest.EFCore.Services
{
    /// <summary>
    /// Topoligical sort service.
    /// </summary>
    public class TopologicalSortService
    {
        /// <summary>
        /// Sorts a collection topoligically.
        /// </summary>
        /// <typeparam name="T">Type of the objects.</typeparam>
        /// <param name="source">Source collection.</param>
        /// <param name="getDependencies">Function to get dependencies of an object.</param>
        public IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies)
        {
            var sorted = new List<T>(source.Count());
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        private void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
        {
            if (visited.TryGetValue(item, out var inProcess))
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found.");
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = getDependencies(item);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        Visit(dependency, getDependencies, sorted, visited);
                    }
                }

#pragma warning disable S4143 // Collection elements should not be replaced unconditionally
                visited[item] = false;
#pragma warning restore S4143 // Collection elements should not be replaced unconditionally
                sorted.Add(item);
            }
        }
    }
}