param($installPath, $toolsPath, $package, $project)

$directory = Split-Path -Path $project.FullName;

$xUnitRunnerPath = Join-Path -Path $directory -ChildPath "xunit.runner.json";

if (!(Test-Path $xUnitRunnerPath))
{
	$value = @'
{
  "parallelizeAssembly": false,
  "parallelizeTestCollections": false
}
'@;

	New-Item -Path $xUnitRunnerPath -ItemType "file" -Value $value
}

$settingsPath = Join-Path -Path $directory -ChildPath "integrationtestsettings.json";

if (!(Test-Path $settingsPath))
{
	$value = @'
{
}
'@;

	New-Item -Path $settingsPath -ItemType "file" -Value $value
}

$xUnitHelpersPath = Join-Path -Path $directory -ChildPath "XunitHelpers.cs";

if (!(Test-Path $xUnitHelpersPath))
{
		$value = @"
using LTest;
using System.Runtime.CompilerServices;

namespace $($project.Name)
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
"@;

	New-Item -Path $xUnitHelpersPath -ItemType "file" -Value $value
}