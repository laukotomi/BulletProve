namespace LTest.Interfaces
{
    /// <summary>
    /// Server log filter.
    /// </summary>
    public interface IServerLogFilter
    {
        /// <summary>
        /// Whether to log a server log in test output.
        /// </summary>
        /// <param name="loggerCategoryName">Logger category name.</param>
        bool Filter(string loggerCategoryName);
    }
}