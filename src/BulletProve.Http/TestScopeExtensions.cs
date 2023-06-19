using BulletProve.Exceptions;
using BulletProve.Http.Models;
using BulletProve.Http.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace BulletProve
{
    /// <summary>
    /// Extensions for integration test service provider.
    /// </summary>
    public static class TestScopeExtensions
    {
        /// <summary>
        /// Creates an HTTP the request for an action.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="actionSelector">The action selector.</param>
        public static HttpRequestBuilder HttpRequestFor<TController>(this ServerScope serviceProvider, Expression<Func<TController, Delegate>> actionSelector)
            where TController : ControllerBase
        {
            var controllerType = typeof(TController);
            var actionName = GetActionName(actionSelector);

            var action = controllerType.GetMethod(actionName);
            if (action == null)
            {
                throw new BulletProveException($"Action '{actionName}' in controller '{controllerType.Name}' can not be found!");
            }

            var httpMethodService = serviceProvider.GetRequiredService<HttpMethodService>();
            var method = httpMethodService.GetHttpMethodForAction(action);

            var linkGeneratorContext = new LinkGeneratorContext(method, controllerType.Name, actionName);

            return new HttpRequestBuilder(linkGeneratorContext, serviceProvider);
        }

        /// <summary>
        /// Gets the action name.
        /// </summary>
        /// <param name="lambda">The lambda.</param>
        /// <returns>A string.</returns>
        private static string GetActionName(LambdaExpression lambda)
        {
            if (lambda.Body is UnaryExpression unary &&
                unary.Operand is MethodCallExpression call &&
                call.Object is ConstantExpression expression &&
                expression.Value is MethodInfo method)
            {
                return method.Name;
            }

            throw new BulletProveException("Invalid action selector used. Use like this: x => x.Action");
        }
    }
}