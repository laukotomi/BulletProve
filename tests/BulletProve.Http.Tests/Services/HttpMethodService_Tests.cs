using BulletProve.Exceptions;
using BulletProve.Http.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BulletProve.Http.Tests.Services
{
    /// <summary>
    /// The http method service tests.
    /// </summary>
    public class HttpMethodService_Tests
    {
        /// <summary>
        /// Tests the get http method for action.
        /// </summary>
        [Fact]
        public void TestGetHttpMethodForAction()
        {
            var sut = new HttpMethodService();
            var method = GetType().GetMethod(nameof(Method), BindingFlags.NonPublic | BindingFlags.Instance);
            var httpMethod = sut.GetHttpMethodForAction(method!);
            httpMethod.Should().Be(HttpMethod.Get);
        }

        /// <summary>
        /// Tests the get http method for action no attribute.
        /// </summary>
        [Fact]
        public void TestGetHttpMethodForActionNoAttribute()
        {
            var sut = new HttpMethodService();
            var method = GetType().GetMethod(nameof(BadMethod), BindingFlags.NonPublic | BindingFlags.Instance);
            var act = () => sut.GetHttpMethodForAction(method!);
            act.Should().Throw<BulletProveException>();
        }

        /// <summary>
        /// Method.
        /// </summary>
        [HttpGet]
        private void Method()
        {
            // Nothing to do
        }

        /// <summary>
        /// Bad method.
        /// </summary>
        [ExcludeFromCodeCoverage]
        private void BadMethod()
        {
            // Nothing to do
        }
    }
}
