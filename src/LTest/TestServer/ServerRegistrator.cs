namespace LTest.TestServer
{
    public class ServerRegistrator
    {
        private readonly TestServerManager _serverManager;

        public ServerRegistrator(TestServerManager serverManager)
        {
            _serverManager = serverManager;
        }

        public void RegisterServer<TServer>(string name)
            where TServer : class, ITestServer, new()
        {
            _serverManager.RegisterServer<TServer>(name);
        }
    }
}
