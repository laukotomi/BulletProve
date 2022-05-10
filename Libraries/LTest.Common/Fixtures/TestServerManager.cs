using LTest.Attributes;
using LTest.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace LTest
{
    /// <summary>
    /// Test server manager.
    /// </summary>
    public class TestServerManager
    {
        private readonly ConcurrentDictionary<Type, ITestServer> _servers = new ConcurrentDictionary<Type, ITestServer>();
        private readonly object _lock = new object();
        private Type _defaultServer;

        /// <summary>
        /// Returns the server for the type.
        /// </summary>
        /// <param name="testServerType">Test server type.</param>
        public ITestServer GetServer(Type testServerType)
        {
            if (testServerType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new ArgumentException($"{testServerType.Name} should have parameterless constructor");
            }

            return _servers.GetOrAdd(testServerType, t =>
            {
                return Activator.CreateInstance(testServerType) as ITestServer;
            });
        }

        /// <summary>
        /// Tries to get the default test server using the <see cref="DefaultTestServerAttribute"/>.
        /// </summary>
        /// <param name="assembly">Assembly to scan.</param>
        public Type TryGetDefaultTestServer(Assembly assembly)
        {
            lock (_lock)
            {
                if (_defaultServer == null)
                {
                    _defaultServer = assembly.GetTypes()
                        .Where(x => x.GetCustomAttribute<DefaultTestServerAttribute>() != null)
                        .FirstOrDefault();
                }

                return _defaultServer;
            }
        }
    }
}