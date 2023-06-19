namespace LTest.Exceptions
{
    /// <summary>
    /// The bullet prove exception.
    /// </summary>
    public class BulletProveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulletProveException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BulletProveException(string? message) : base(message) { }

        public BulletProveException(string? message, Exception inner) : base(message, inner) { }
    }
}
