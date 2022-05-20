using System.Reflection;
using Xunit.Abstractions;

namespace LTest.Helpers
{
    /// <summary>
    /// The xunit reflection helper.
    /// </summary>
    internal static class XunitReflectionHelper
    {
        /// <summary>
        /// Gets the test context.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <returns>An ITest object.</returns>
        public static ITest GetTestContext(ITestOutputHelper output)
        {
            var testOutputType = output.GetType();
            var testField = testOutputType.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            if (testField != null)
            {
                if (testField.GetValue(output) is ITest context)
                {
                    return context;
                }
            }

            throw new InvalidOperationException("Unable to get test context. Probably xunit library was updated.");
        }

        /// <summary>
        /// Tries the get test server type.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns>A Type.</returns>
        public static Type? TryGetTestServerType(ITest testContext)
        {
            var testCase = testContext.TestCase;

            // Try get from method parameter
            var serverType = typeof(ITestServer);
            var type = testCase.TestMethodArguments?.OfType<Type>().FirstOrDefault(x => serverType.IsAssignableFrom(x));
            if (type != null)
            {
                return type;
            }

            var attributeAssemblyQualifiedName = typeof(TestServerAttribute).AssemblyQualifiedName;

            // Try get from method attribute
            var method = testCase.TestMethod.Method;
            var attribute = method
                .GetCustomAttributes(attributeAssemblyQualifiedName)
                .FirstOrDefault();

            if (attribute != null)
            {
                return GetTypeFromAttribute(attribute);
            }

            // Try get from class attribute
            var testClass = testCase.TestMethod.TestClass.Class;
            attribute = testClass
                .GetCustomAttributes(attributeAssemblyQualifiedName)
                .FirstOrDefault();

            if (attribute != null)
            {
                return GetTypeFromAttribute(attribute);
            }

            return type;
        }

        /// <summary>
        /// Gets the type object from attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>A Type.</returns>
        private static Type GetTypeFromAttribute(IAttributeInfo attribute)
        {
            var attributeProperty = attribute.GetType().GetProperty("Attribute");
            if (attributeProperty == null)
            {
                throw new InvalidOperationException($"Unable to get {nameof(TestServerAttribute)}. Probably xunit library was updated.");
            }

            var testServerAttribute = (TestServerAttribute)attributeProperty.GetValue(attribute)!;
            return testServerAttribute.TestServerType;
        }
    }
}
