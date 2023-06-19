using BulletProve;
using System.Runtime.CompilerServices;

namespace Example.Api.IntegrationTests
{
    /// <summary>
    /// The theory attribute.
    /// </summary>
    public class TheoryAttribute : Xunit.TheoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoryAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        public TheoryAttribute([CallerMemberName] string? memberName = null)
        {
            DisplayName = memberName;
        }
    }

    /// <summary>
    /// The fact attribute.
    /// </summary>
    public class FactAttribute : Xunit.FactAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FactAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The member name.</param>
        public FactAttribute([CallerMemberName] string? memberName = null)
        {
            DisplayName = memberName;
        }
    }

    /// <summary>
    /// The integration test collection.
    /// </summary>
    [Xunit.CollectionDefinition("Integration Tests")]
    public class IntegrationTestCollection : Xunit.ICollectionFixture<TestServerManager>
    {
    }
}