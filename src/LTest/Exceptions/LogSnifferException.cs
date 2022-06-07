namespace LTest.Exceptions
{
    /// <summary>
    /// The log sniffer exception.
    /// </summary>
    public class LogSnifferException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogSnifferException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogSnifferException(string? message) : base(message)
        {
        }
    }
}
