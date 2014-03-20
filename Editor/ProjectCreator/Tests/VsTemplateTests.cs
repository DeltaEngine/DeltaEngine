using System.IO;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Visual Studio Template.
	/// </summary>
	[Category("Slow")]
	public class VsTemplateTests
	{
		[Test]
		public void CreateWithEmptyAppTemplate()
		{
			var template = new VsTemplate("EmptyApp");
			Assert.IsTrue(template.PathToZip.EndsWith("EmptyApp.zip"));
			Assert.IsTrue(template.AssemblyInfo.EndsWith(Path.Combine("Properties", "AssemblyInfo.cs")));
			Assert.IsTrue(template.Csproj.EndsWith("EmptyApp.csproj"));
			Assert.IsTrue(template.Icons[0].EndsWith("EmptyApp.ico"));
			Assert.AreEqual(2, template.SourceCodeFiles.Count);
			Assert.IsTrue(template.SourceCodeFiles[0].EndsWith("ColorChanger.cs"));
			Assert.IsTrue(template.SourceCodeFiles[1].EndsWith("Program.cs"));
		}

		[Test]
		public void CheckTotalNumberOfFilesFromEmptyAppTemplate()
		{
			var template = new VsTemplate("EmptyApp");
			var list = template.GetAllFilePathsAsList();
			Assert.AreEqual(5, list.Count);
		}

		[Test]
		public void VsTemplatesHaveToBeAvailable()
		{
			Assert.Greater(VsTemplate.GetAllTemplateNames(DeltaEngineFramework.GLFW).Length, 0);
		}
	}
}