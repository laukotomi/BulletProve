using System;

namespace LTest
{
    /// <summary>
    /// Test server attribute.
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
            if (testServerType == null)
            {
                throw new ArgumentNullException(nameof(testServerType));
            }

            TestServerType = testServerType;
        }

        /// <summary>
        /// Test server type.
        /// </summary>
        public Type TestServerType { get; }
    }
}