using System.IO;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the creation of Delta Engine C# projects.
	/// </summary>
	[Category("Slow")]
	public class ProjectCreatorTests
	{
		[SetUp]
		public void Init()
		{
			valid = CreateWithValidFileSystemMock();
			invalid = CreateWithCorruptFileSystemMock();
		}

		private ProjectCreator valid;
		private ProjectCreator invalid;

		private static ProjectCreator CreateWithValidFileSystemMock()
		{
			var project = new CsProject("John Doe");
			var template = new VsTemplate("EmptyApp");
			return new ProjectCreator(project, template, CreateMockService());
		}

		private static Service CreateMockService()
		{
			return new MockService("John Doe", "LogoApp");
		}

		private static ProjectCreator CreateWithCorruptFileSystemMock()
		{
			var template = new VsTemplate("EmptyApp");
			return new ProjectCreator(new CsProject(""), template, CreateMockService());
		}

		[Test]
		public void CheckAvailabilityOfTheTemplateFiles()
		{
			Assert.IsTrue(valid.AreAllTemplateFilesAvailable());
			Assert.IsFalse(invalid.AreAllTemplateFilesAvailable());
		}

		[Test]
		public void CheckAvailabilityOfTheSourceFile()
		{
			Assert.IsTrue(valid.IsSourceFileAvailable());
			Assert.IsFalse(invalid.IsSourceFileAvailable());
		}

		[Test]
		public void CheckIfFolderHierarchyIsCreatedCorrectly()
		{
			valid.CreateProject();
			Assert.IsTrue(valid.HasDirectoryHierarchyBeenCreated());
			invalid.CreateProject();
			Assert.IsFalse(invalid.HasDirectoryHierarchyBeenCreated());
		}

		[Test]
		public void CheckIfProjectFilesAreCopiedCorrectly()
		{
			valid.CreateProject();
			valid.CheckIfTemplateFilesHaveBeenCopiedToLocation();
			invalid.CreateProject();
			Assert.Throws<FileNotFoundException>(
				() => invalid.CheckIfTemplateFilesHaveBeenCopiedToLocation());
		}
	}
}