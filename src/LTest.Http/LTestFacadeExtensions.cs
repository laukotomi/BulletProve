using LTest.Http.Models;
using LTest.Http.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace LTest
{
    /// <summary>
    /// Extensions for integration test service provider.
    /// </summary>
    public static class LTestFacadeExtensions
    {
        public static HttpRequestBuilder HttpRequestFor<TController>(this LTestFacade serviceProvider, Expression<Func<TController, Delegate>> actionSelector)
            where TController : ControllerBase
        {
            var controllerType = typeof(TController);
            var actionName = GetActionName(actionSelector);

            var action = controllerType.GetMethod(actionName);
            if (action == null)
            {
                throw new InvalidOperationException($"Action '{actionName}' in controller '{controllerType.Name}' can not be found!");
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

            throw new InvalidOperationException("Invalid action selector used. Use like this: x => x.Action");
        }
    }
}