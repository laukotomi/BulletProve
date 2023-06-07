namespace LTest.Http.Models
{
    public class LinkGeneratorContext
    {
        public string ControllerName { get; }
        public string ActionName { get; }
        public HttpMethod Method { get; }
        public Dictionary<string, string> UriValues { get; } = new();

        public LinkGeneratorContext(HttpMethod method, string controllerName, string actionName)
        {
            Method = method;
            ControllerName = controllerName;
            ActionName = actionName;
        }
    }
}
