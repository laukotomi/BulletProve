namespace LTest.Interfaces
{
    /// <summary>
    /// Clear method will be called before test.
    /// </summary>
    public interface ICleanSingletonBeforeTest
    {
        /// <summary>
        /// Clean up.
        /// </summary>
        void Clear();
    }
}