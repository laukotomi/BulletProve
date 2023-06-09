namespace LTest.TestServer
{
    public interface IServerRegistrator
    {
        public void RegisterServer<TStartup>(string serverName, Action<ServerConfigurator>? configAction = null)
            where TStartup : class;
    }
}
