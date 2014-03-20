using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class CodePackerTests
	{
		[Test]
		public void ProjectNotFoundInSolutionException()
		{
			Assert.Throws<SolutionFileLoader.ProjectNotFoundInSolution>(
				() => new CodePacker(PathExtensions.GetDeltaEngineSolutionFilePath(), "App"));
		}

		[Test]
		public void ExpectExceptionWhenPackDirectoryWithoutCode()
		{
			const string Folder = "ExpectExceptionWhenPackEmptyFolder";
			try
			{
				Directory.CreateDirectory(Folder);
				Assert.Throws<CodePacker.NoCodeAvailableToPack>(() => new CodePacker(Folder));
			}
			finally
			{
				if (Directory.Exists(Folder))
					Directory.Delete(Folder);
			}
		}

		// ncrunch: no coverage start
		[Test, Category("Slow")]
		public void LoadCodeFromProject()
		{
			CodePacker packer = GetCodePackerWithBuilderTestsData();
			Assert.AreNotEqual(0, packer.CollectedFilesToPack.Count);
		}

		private static CodePacker GetCodePackerWithBuilderTestsData()
		{
			string solutionFilePath = PathExtensions.GetDeltaEngineSolutionFilePath();
			string testsProjectName = Path.GetFileNameWithoutExtension(solutionFilePath);
			return new CodePacker(solutionFilePath, testsProjectName);
		}
	}
}