namespace BulletProve.Exceptions
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

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletProveException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public BulletProveException(string? message, Exception innerException) : base(message, innerException) { }
    }
}
