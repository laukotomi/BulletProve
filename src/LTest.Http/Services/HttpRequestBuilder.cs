using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Reflection;

namespace LTest.Http.Services
{
    /// <summary>
    /// Builder for HTTP requests.
    /// </summary>
    public class HttpRequestBuilder
    {
        private readonly HttpMethodService _httpMethodService;
        private readonly LTestFacade _facade;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class.
        /// </summary>
        /// <param name="httpMethodService"><see cref="HttpMethodService"/> object.</param>
        /// <param name="facade">Service provider.</param>
        public HttpRequestBuilder(
            HttpMethodService httpMethodService,
            LTestFacade facade)
        {
            _httpMethodService = httpMethodService;
            _facade = facade;
        }

        /// <summary>
        /// Creates a HTTP request object for a controller's action.
        /// </summary>
        /// <typeparam name="TController">Type of the controller.</typeparam>
        /// <param name="actionNameSelector">A lambda that returns the nameof(Action).</param>
        public HttpRequestService CreateFor<TController>(Func<TController?, string> actionNameSelector)
            where TController : ControllerBase
        {
            var actionName = actionNameSelector(null);
            return CreateHttpRequestService(typeof(TController), actionName);
        }

        /// <summary>
        /// Creates a HTTP request object for a controller's action.
        /// </summary>
        /// <typeparam name="TController">Type of the controller.</typeparam>
        /// <param name="actionSelector">A lambda that returns the action.</param>
        public HttpRequestService CreateFor<TController>(Expression<Func<TController, Delegate>> actionSelector)
            where TController : ControllerBase
        {
            var actionName = GetActionName(actionSelector);
            return CreateHttpRequestService(typeof(TController), actionName);
        }

        /// <summary>
        /// Creates the http request service.
        /// </summary>
        /// <param name="controllerType">The controller type.</param>
        /// <param name="actionName">The action name.</param>
        /// <returns>A HttpRequestService.</returns>
        private HttpRequestService CreateHttpRequestService(Type controllerType, string actionName)
        {
            var action = controllerType.GetMethod(actionName);
            if (action == null)
            {
                throw new InvalidOperationException($"Action '{actionName} in controller '{controllerType.Name}' can not be found!");
            }

            var method = _httpMethodService.GetHttpMethodForAction(action);

            return new HttpRequestService(controllerType.Name, actionName, method, _facade);
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

    /// <summary>
    /// Builder for HTTP requests.
    /// </summary>
    /// <typeparam name="TController">Type of the controller.</typeparam>
    public class HttpRequestBuilder<TController> : HttpRequestBuilder
            where TController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder{TController>"/> class.
        /// </summary>
        /// <param name="httpMethodService"><see cref="HttpMethodService"/> object.</param>
        /// <param name="facade">Service provider.</param>
        public HttpRequestBuilder(
            HttpMethodService httpMethodService,
            LTestFacade facade)
            : base(httpMethodService, facade)
        {
        }

        /// <summary>
        /// Creates a HTTP request object for a controller's action.
        /// </summary>
        /// <param name="actionSelector">A lambda that returns the action.</param>
        public HttpRequestService CreateFor(Expression<Func<TController, Delegate>> actionSelector)
        {
            return CreateFor<TController>(actionSelector);
        }

        /// <summary>
        /// Creates a HTTP request object for a controller's action.
        /// </summary>
        /// <param name="actionNameSelector">A lambda that returns the nameof(Action).</param>
        public HttpRequestService CreateFor(Func<TController?, string> actionNameSelector)
        {
            return CreateFor<TController>(actionNameSelector);
        }
    }
}