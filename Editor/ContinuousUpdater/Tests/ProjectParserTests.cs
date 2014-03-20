using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContinuousUpdater.Tests
{
	public class ProjectParserTests
	{
		[SetUp]
		public void CreateParser()
		{
			parser = StackTraceExtensions.IsStartedFromNCrunch()
				? new MockProjectParser() : new ProjectParser();
		}

		private ProjectParser parser;

		[Test]
		public void EmptyDirectoryShouldNotCheckAnyDlls()
		{
			var projects = parser.Parse(@"C:\");
			Assert.AreEqual(0, parser.AssembliesChecked);
			Assert.AreEqual(0, parser.AssembliesMatching);
			Assert.AreEqual(0, projects.Count);
		}

		[Test]
		public void DirectoryWithoutTestsOrMain()
		{
			var projects = parser.Parse(@"C:\code\DeltaEngine\bin\Debug");
			Assert.AreEqual(0, parser.AssembliesChecked);
			Assert.AreEqual(0, parser.AssembliesMatching);
			Assert.AreEqual(0, projects.Count);
		}

		[Test, Ignore]
		public void DirectoryWithoutVisualEntryPoints()
		{
			var projects = parser.Parse(@"C:\code\DeltaEngine\Tests\bin\Debug");
			Assert.GreaterOrEqual(parser.AssembliesChecked, 5);
			Assert.AreEqual(0, parser.AssembliesMatching);
			Assert.AreEqual(0, projects.Count);
		}

		[Test, Ignore]
		public void OnlyOneDllInPathShouldBeChecked()
		{
			var projects = parser.Parse(@"C:\code\DeltaEngine\Samples\LogoApp\Tests\bin\Debug");
			Assert.Greater(parser.AssembliesChecked, 5);
			Assert.AreEqual(1, parser.AssembliesMatching);
			Assert.AreEqual(1, projects.Count);
		}

		[Test, Ignore]
		public void LookForLogoAppExe()
		{
			var projects = parser.Parse(@"C:\code\DeltaEngine\Samples\LogoApp\bin\Debug");
			Assert.Greater(parser.AssembliesChecked, 5);
			Assert.AreEqual(1, parser.AssembliesMatching);
			Assert.AreEqual(1, projects.Count);
		}
	}
}