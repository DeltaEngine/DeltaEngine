using NUnit.Framework;

namespace DeltaEngine.Editor.ContinuousUpdater.Tests
{
	public class ProjectTests
	{
		[TestCase(@"C:\", 0)]
		[TestCase(@"C:\code\DeltaEngine\bin\Debug", 0)]
		[TestCase(@"C:\code\DeltaEngine\Tests\bin\Debug", 0)]
		[TestCase(@"C:\code\DeltaEngine\Graphics\Tests\bin\Debug", 1)]
		[TestCase(@"C:\code\DeltaEngine\Samples\LogoApp\bin\Debug", 1)]
		[TestCase(@"C:\code\DeltaEngine\Samples\LogoApp\Tests\bin\Debug", 1)]
		public void ExpectToFindAssembliesWithEntryPointsInProjectPath(string path, int count)
		{
			new Project(path);
		}
	}
}