using System;
using System.IO;
using DeltaEngine.Editor.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the Delta Engine C# project.
	/// </summary>
	public class CsProjectTests
	{
		[SetUp]
		public void Init()
		{
			project = new CsProject("User Name");
		}

		private CsProject project;

		[Test]
		public void DefaultStarterKitIsEmptyApp()
		{
			Assert.AreEqual("EmptyApp", project.StarterKit);
		}

		[Test]
		public void DefaultProjectNameIsUserNamePlusStarterKitName()
		{
			Assert.AreEqual("UserNamesEmptyApp", project.Name);
		}

		[Test]
		public void ChangingStarterKitOnlyChangesTheNameIfItHasNotBeenChangedYet()
		{
			project.StarterKit = "LogoApp";
			Assert.AreEqual("UserNamesLogoApp", project.Name);
			project.StarterKit = "Snake";
			Assert.AreEqual("UserNamesSnake", project.Name);
			project.Name = "GeneratedEmptyApp";
			Assert.AreEqual("GeneratedEmptyApp", project.Name);
			project.StarterKit = "Asteroids";
			Assert.AreEqual("GeneratedEmptyApp", project.Name);
		}

		[Test]
		public void UserNameEndingWithSDoesNotAppendAdditionalS()
		{
			project = new CsProject("Ellis");
			Assert.AreEqual("EllisEmptyApp", project.Name);
			project = new CsProject("ElviS");
			Assert.AreEqual("ElviSEmptyApp", project.Name);
		}

		[Test]
		public void DefaultFrameworkIsOpenTK()
		{
			Assert.AreEqual(DeltaEngineFramework.OpenTK, project.Framework);
		}

		[Test]
		public void DefaultPathIsDefaultVisualStudioProjectLocation()
		{
			Assert.IsTrue(project.BaseDirectory.Contains("Visual Studio") &&
				project.BaseDirectory.Contains("Projects"));
		}

		[Test]
		public void VisualStudioProjectsFolderInMyDocumentsMustBeAvailable()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			Assert.IsTrue(project.BaseDirectory.StartsWith(myDocumentsPath));
			Assert.IsTrue(project.BaseDirectory.Contains("Visual Studio"));
			Assert.IsTrue(project.BaseDirectory.Contains("Projects"));
		}

		[Test]
		public void OutputDirectoryShouldBeBaseDirectoryPlusProjectName()
		{
			string myDocumentsPath = MyDocumentsExtensions.GetVisualStudioProjectsFolder();
			Assert.AreEqual(Path.Combine(myDocumentsPath, "UserNamesEmptyApp"), project.OutputDirectory);
			project.StarterKit = "LogoApp";
			Assert.AreEqual(Path.Combine(myDocumentsPath, "UserNamesLogoApp"), project.OutputDirectory);
			project.Name = "MyEmptyApp";
			Assert.AreEqual(Path.Combine(myDocumentsPath, "MyEmptyApp"), project.OutputDirectory);
		}
	}
}