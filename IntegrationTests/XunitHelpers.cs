using LTest;
using System.Runtime.CompilerServices;

namespace IntegrationTests
{
    public class TheoryAttribute : Xunit.TheoryAttribute
    {
        public TheoryAttribute([CallerMemberName] string memberName = null)
        {
            DisplayName = memberName;
        }
    }

    public class FactAttribute : Xunit.FactAttribute
    {
        public FactAttribute([CallerMemberName] string memberName = null)
        {
            DisplayName = memberName;
        }
    }

    [Xunit.CollectionDefinition("Integration Test")]
    public class IntegrationTestCollection : Xunit.ICollectionFixture<TestServerManager>
    {
    }
}