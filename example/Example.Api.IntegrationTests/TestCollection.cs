namespace BulletProve
{
    /// <summary>
    /// The integration test collection.
    /// </summary>
    [Xunit.CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : Xunit.ICollectionFixture<TestServerManager>
    {
    }
}