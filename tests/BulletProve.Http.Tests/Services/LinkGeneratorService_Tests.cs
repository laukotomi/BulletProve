using BulletProve.Exceptions;
using BulletProve.Http.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The link generator service tests.
    /// </summary>
    public class LinkGeneratorService_Tests
    {
        /// <summary>
        /// Tests the get request uri.
        /// </summary>
        [Fact]
        public void TestGetRequestUri()
        {
            var sut = new LinkGeneratorService(new TestLinkGenerator());
            var uri = sut.GetRequestUri("ActionAsync", "SuperController", new()
            {
                { "asd", "dsa" }
            });
            uri.ToString().Should().Be("https://localhost/(asd=[dsa],action=[Action],controller=[Super])");
        }

        /// <summary>
        /// Tests the get request uri missing parameter.
        /// </summary>
        [Fact]
        public void TestGetRequestUriMissingParameter()
        {
            var sut = new LinkGeneratorService(new TestLinkGenerator());
            var act = () => sut.GetRequestUri("action", "contr");
            act.Should().Throw<BulletProveException>();
        }

        /// <summary>
        /// The test link generator.
        /// </summary>
        private class TestLinkGenerator : LinkGenerator
        {
            /// <inheritdoc/>
            public override string? GetPathByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override string? GetPathByAddress<TAddress>(TAddress address, RouteValueDictionary values, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
            {
                if (!address!.ToString()!.Contains("asd"))
                    return null;

                return $"https://localhost/{address}";
            }

            /// <inheritdoc/>
            public override string? GetUriByAddress<TAddress>(HttpContext httpContext, TAddress address, RouteValueDictionary values, RouteValueDictionary? ambientValues = null, string? scheme = null, HostString? host = null, PathString? pathBase = null, FragmentString fragment = default, LinkOptions? options = null)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc/>
            public override string? GetUriByAddress<TAddress>(TAddress address, RouteValueDictionary values, string scheme, HostString host, PathString pathBase = default, FragmentString fragment = default, LinkOptions? options = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
