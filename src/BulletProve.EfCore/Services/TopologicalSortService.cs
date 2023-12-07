namespace BulletProve.EfCore.Services
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
        public IList<T> Sort<T>(ICollection<T> source, Func<T, IEnumerable<T>> getDependencies)
            where T : notnull
        {
            var sorted = new List<T>(source.Count);
            var visited = new Dictionary<T, bool>();

            foreach (var item in source)
            {
                Visit(item, getDependencies, sorted, visited);
            }

            return sorted;
        }

        /// <summary>
        /// Visitor.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="getDependencies">The get dependencies.</param>
        /// <param name="sorted">The sorted.</param>
        /// <param name="visited">The visited.</param>
        private void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
            where T : notnull
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

                if (visited[item])
                {
                    visited[item] = false;
                }
                sorted.Add(item);
            }
        }
    }
}