namespace LTest
{
    /// <summary>
    /// Test server attribute. Specifies the type of the server to use for a specific test method or test class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TestServerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestServerAttribute"/> class.
        /// </summary>
        /// <param name="testServerType">Test server type.</param>
        public TestServerAttribute(Type testServerType)
        {
            TestServerType = testServerType ?? throw new ArgumentNullException(nameof(testServerType));
        }

        /// <summary>
        /// Test server type.
        /// </summary>
        public Type TestServerType { get; }
    }
}