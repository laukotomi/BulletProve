using BulletProve.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace BulletProve.Tests.TestServer
{
    /// <summary>
    /// The test server manager tests.
    /// </summary>
    public class TestServerManager_Tests
    {
        private readonly TestServerManager _sut;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestServerManager_Tests"/> class.
        /// </summary>
        public TestServerManager_Tests()
        {
            _sut = new TestServerManager();
        }

        /// <summary>
        /// Tests the register server.
        /// </summary>
        [Fact]
        public void TestRegisterServer()
        {
            _sut.RegisterServer<MyServer>("name", x => x.MinimumLogLevel = LogLevel.Error);

            var server = _sut.GetServer("name");
            server.Should().NotBeNull();
            server.Should().BeOfType<TestServer<MyServer>>();
        }

        /// <summary>
        /// Tests the without server name.
        /// </summary>
        [Fact]
        public void TestWithoutServerName()
        {
            var act = () => _sut.RegisterServer<MyServer>(string.Empty);
            act.Should().Throw<BulletProveException>();
        }

        /// <summary>
        /// Tests the already registered server.
        /// </summary>
        [Fact]
        public void TestAlreadyRegisteredServer()
        {
            _sut.RegisterServer<MyServer>("name");
            var act = () => _sut.RegisterServer<MyServer>("name");
            act.Should().Throw<BulletProveException>();
        }

        /// <summary>
        /// Tests the get server bad name.
        /// </summary>
        [Fact]
        public void TestGetServerBadName()
        {
            var act = () => _sut.GetServer("aaa");
            act.Should().Throw<BulletProveException>();
        }

        /// <summary>
        /// Tests the has servers.
        /// </summary>
        [Fact]
        public void TestHasServers()
        {
            _sut.HasServers.Should().BeFalse();
            _sut.RegisterServer<MyServer>("name");
            _sut.HasServers.Should().BeTrue();
        }

        /// <summary>
        /// Tests the dispose.
        /// </summary>
        [Fact]
        public void TestDispose()
        {
            _sut.RegisterServer<MyServer>("name");
            var server = _sut.GetServer("name");
            _sut.Dispose();
            server.IsDisposed.Should().BeTrue();
        }

        /// <summary>
        /// The my server.
        /// </summary>
        private sealed class MyServer : IDisposable
        {
            /// <summary>
            /// Gets a value indicating whether is disposed.
            /// </summary>
            public bool IsDisposed { get; private set; }

            /// <inheritdoc/>
            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
