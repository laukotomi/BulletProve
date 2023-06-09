namespace LTest.Exceptions
{
    /// <summary>
    /// The log sniffer exception.
    /// </summary>
    public class BulletProveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletProveException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BulletProveException(string? message) : base(message)
        {
        }
    }
}
