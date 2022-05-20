using LTest.Helpers;
using System.Reflection;
using Xunit.Abstractions;

namespace LTest
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public class TestServerManager
    {
        private readonly Dictionary<Type, ITestServer> _servers = new();
        private Type? _defaultServer;

        /// <summary>
        /// Gets the test server.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="testSuite">The test suite.</param>
        /// <param name="created">Whether the server just created.</param>
        /// <returns>An ITestServer.</returns>
        internal ITestServer GetServer(ITest testContext, Type testSuite, out bool created)
        {
            var serverType = XunitReflectionHelper.TryGetTestServerType(testContext) ?? TryGetDefaultTestServer(testSuite.Assembly);

            if (serverType == null)
            {
                throw new InvalidOperationException($"Could not find test server for {testSuite.Name}/{testContext.DisplayName}");
            }

            if (!typeof(ITestServer).IsAssignableFrom(serverType))
            {
                throw new ArgumentException($"{serverType.Name} does not implement {nameof(ITestServer)}");
            }

            if (serverType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"{serverType.Name} should contain parameterless constructor");
            }

            if (!_servers.TryGetValue(serverType, out var server))
            {
                server = Activator.CreateInstance(serverType) as ITestServer;
                _servers.Add(serverType, server!);
                created = true;
            }
            else
            {
                created = false;
            }

            return server!;
        }

        /// <summary>
        /// Tries to get the default test server using the <see cref="DefaultTestServerAttribute"/>.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        private Type? TryGetDefaultTestServer(Assembly assembly)
        {
            if (_defaultServer == null)
            {
                var defaultServers = assembly.GetTypes()
                    .Where(x => x.GetCustomAttribute<DefaultTestServerAttribute>() != null)
                    .ToList();

                if (defaultServers.Count == 1)
                {
                    _defaultServer = defaultServers[0];
                }
                else if (defaultServers.Count > 1)
                {
                    throw new InvalidOperationException($"More than one default servers found! Default servers: {string.Join(", ", defaultServers.Select(x => x.Name))}");
                }
            }

            return _defaultServer;
        }
    }
}