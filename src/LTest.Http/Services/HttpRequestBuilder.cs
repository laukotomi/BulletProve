using Microsoft.AspNetCore.Mvc;

namespace LTest.Http.Services
{
    /// <summary>
    /// Builder for HTTP requests.
    /// </summary>
    public class HttpRequestBuilder
    {
        private readonly HttpMethodService _httpMethodService;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestBuilder"/> class.
        /// </summary>
        /// <param name="httpMethodService"><see cref="HttpMethodService"/> object.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public HttpRequestBuilder(
            HttpMethodService httpMethodService,
            IServiceProvider serviceProvider)
        {
            _httpMethodService = httpMethodService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Creates a HTTP request object for a controller's action.
        /// </summary>
        /// <typeparam name="TController">Type of the controller.</typeparam>
        /// <param name="actionNameSelector">A lambda that returns the nameof(Action).</param>
        public HttpRequestService CreateFor<TController>(Func<TController?, string> actionNameSelector)
            where TController : ControllerBase
        {
            var controllerType = typeof(TController);
            var actionName = actionNameSelector(null);
            var action = controllerType.GetMethod(actionName);
            if (action == null)
            {
                throw new InvalidOperationException($"Action '{actionName} in controller '{controllerType.Name}' can not be found!");
            }

            var method = _httpMethodService.GetHttpMethodForAction(action);

            return new HttpRequestService(controllerType.Name, actionName, method, _serviceProvider);
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
        /// <param name="serviceProvider">Service provider.</param>
        public HttpRequestBuilder(
            HttpMethodService httpMethodService,
            IServiceProvider serviceProvider)
            : base(httpMethodService, serviceProvider)
        {
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