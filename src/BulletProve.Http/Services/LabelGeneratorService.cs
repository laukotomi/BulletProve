namespace BulletProve.Http.Services
{
    /// <summary>
    /// The label generator service.
    /// </summary>
    internal class LabelGeneratorService
    {
        private int _counter = 0;

        /// <summary>
        /// Gets the label.
        /// </summary>
        /// <returns>A string.</returns>
        public string GetLabel()
        {
            return $"#{Interlocked.Increment(ref _counter)}";
        }
    }
}
