using LTest.Helpers;
using LTest.Interfaces;
using LTest.Logger;
using LTest.LogSniffer;
using LTest.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// Base class for integration tests.
    /// </summary>
    [Collection("Integration Test")]
    public abstract class IntegrationTestBase : IDisposable
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestBase"/> class.
        /// </summary>
        /// <param name="serverManager">Test server manager.</param>
        /// <param name="output"><see cref="ITestOutputHelper"/> object.</param>
        protected IntegrationTestBase(TestServerManager serverManager, ITestOutputHelper output)
        {
            _output = output;
            TestContext = GetTestContext(output);

            var serverType = TryGetTestServerType(serverManager);
            if (serverType == null)
            {
                throw new InvalidOperationException($"Could not determine test server");
            }
            if (!typeof(ITestServer).IsAssignableFrom(serverType))
            {
                throw new ArgumentException($"{serverType.Name} should implement {nameof(ITestServer)}.");
            }

            var server = serverManager.GetServer(serverType);
            var serverDetails = InitServer(server);
            Services = serverDetails.Services;
            Logger = serverDetails.Services.Logger;
        }

        /// <summary>
        /// Access services using this property.
        /// </summary>
        protected IntegrationTestServiceProvider Services { get; }

        /// <summary>
        /// Test context.
        /// </summary>
        protected ITest TestContext { get; }

        /// <summary>
        /// Test logger.
        /// </summary>
        protected ITestLogger Logger { get; }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <see cref="IDisposable"/> implementation.
        /// </summary>
        /// <param name="disposing">Disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServicesHelper.RunServices<IAfterTestBehavior>(Services);
                LogLoggerCategoryNames();
                FlushLogger();

                if (Services.LogSniffer.UnexpectedLogOccured)
                {
                    throw new InvalidOperationException($"Unexpected log occured on server side while sending the request. Check the logs!");
                }
            }
        }

        private void FlushLogger()
        {
            Logger.Info("Test finished");

            var logger = (TestLogger)Logger;
            var logEvents = logger.GetSnapshot();
            foreach (var logEvent in logEvents)
            {
                _output.WriteLine(logEvent.ToString());
            }
        }

        private void LogLoggerCategoryNames()
        {
            if (Services.Configuration.LogLoggerCategoryNames)
            {
                var categoryNameCollector = Services.GetRequiredService<CategoryNameCollector>();

                using var loggerScope = Logger.Scope(logger => logger.Info("Logger category names"));
                foreach (var categoryName in categoryNameCollector.CategoryNames)
                {
                    Logger.Info(categoryName);
                }
            }
        }

        private ServerDetails InitServer(ITestServer server)
        {
            bool started = server.EnsureServerStarted();
            var serverDetails = new ServerDetails(server);

            if (!started) // to not clear server startup logs
            {
                serverDetails.Services.Logger.Reset();
                serverDetails.Services.LogSniffer.Reset();
            }

            ServicesHelper.RunServices<IBeforeTestBehavior>(serverDetails.Services);

            return serverDetails;
        }

        private Type TryGetTestServerType(TestServerManager serverManager)
        {
            var testCase = TestContext.TestCase;

            // Try get from method parameter
            var serverType = typeof(ITestServer);
            var type = testCase.TestMethodArguments?.OfType<Type>().FirstOrDefault(x => serverType.IsAssignableFrom(x));
            if (type != null)
            {
                return type;
            }

            // Try get from method attribute
            var method = testCase.TestMethod.Method;
            var attribute = method
                .GetCustomAttributes(typeof(TestServerAttribute).AssemblyQualifiedName)
                .FirstOrDefault();

            if (attribute != null)
            {
                var attributeProperty = attribute.GetType().GetProperty("Attribute");
                if (attributeProperty == null)
                {
                    throw new InvalidOperationException($"Unable to get {nameof(TestServerAttribute)}. Probably xunit library was updated.");
                }

                return (attributeProperty.GetValue(attribute) as TestServerAttribute)?.TestServerType;
            }

            // Try get from class attribute
            var testType = GetType();
            type = testType.GetCustomAttribute<TestServerAttribute>()?.TestServerType;
            if (type != null)
            {
                return type;
            }

            return serverManager.TryGetDefaultTestServer(testType.Assembly);
        }

        private ITest GetTestContext(ITestOutputHelper output)
        {
            var testOutputType = output.GetType();
            var testField = testOutputType.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            if (testField != null)
            {
                var context = testField.GetValue(output) as ITest;
                if (context != null)
                {
                    return context;
                }
            }

            throw new InvalidOperationException("Unable to get test context. Probably xunit library was updated.");
        }
    }
}