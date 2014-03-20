using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the logic of the project creation editor plugin.
	/// </summary>
	[Category("Slow")]
	public class ProjectCreatorViewModelTests
	{
		[SetUp]
		public void Create()
		{
			mockService = new MockService("John Doe", "LogoApp");
			viewModel = new ProjectCreatorViewModel(mockService);
		}

		private MockService mockService;
		private ProjectCreatorViewModel viewModel;

		[Test]
		public void ChangeName()
		{
			const string NewName = "ChangedProjectName";
			viewModel.OnNameChanged.Execute(NewName);
			Assert.AreEqual(NewName, viewModel.Name);
		}
		
		[Test]
		public void CheckAvailableFrameworks()
		{
			Assert.AreEqual(DeltaEngineFramework.GLFW, viewModel.AvailableFrameworks[0]);
			Assert.AreEqual(DeltaEngineFramework.MonoGame, viewModel.AvailableFrameworks[1]);
			Assert.AreEqual(DeltaEngineFramework.OpenTK, viewModel.AvailableFrameworks[2]);
			Assert.AreEqual(DeltaEngineFramework.SharpDX, viewModel.AvailableFrameworks[3]);
			Assert.AreEqual(DeltaEngineFramework.SlimDX, viewModel.AvailableFrameworks[4]);
			Assert.AreEqual(DeltaEngineFramework.Xna, viewModel.AvailableFrameworks[5]);
		}
		
		[TestCase(DeltaEngineFramework.GLFW)]
		[TestCase(DeltaEngineFramework.MonoGame)]
		[TestCase(DeltaEngineFramework.OpenTK)]
		[TestCase(DeltaEngineFramework.SharpDX)]
		[TestCase(DeltaEngineFramework.SlimDX)]
		[TestCase(DeltaEngineFramework.Xna)]
		public void ChangeSelection(DeltaEngineFramework expectedFramework)
		{
			viewModel.OnFrameworkSelectionChanged.Execute(expectedFramework);
			Assert.AreEqual(expectedFramework, viewModel.SelectedFramework);
		}
		
		[Test]
		public void ChangePath()
		{
			const string NewPath = "C:\\DeltaEngine\\";
			viewModel.OnLocationChanged.Execute(NewPath);
			Assert.AreEqual(NewPath, viewModel.Location);
		}

		[Test]
		public void CanCreateProjectWithValidName()
		{
			viewModel.OnNameChanged.Execute("ValidProjectName");
			Assert.IsTrue(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CannotCreateProjectWithInvalidName()
		{
			viewModel.OnNameChanged.Execute("Invalid Project Name");
			Assert.IsFalse(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CanCreateProjectWithValidLocation()
		{
			viewModel.OnLocationChanged.Execute("C:\\ValidLocation\\");
			Assert.IsTrue(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void CannotCreateProjectWithInvalidLocation()
		{
			viewModel.OnLocationChanged.Execute("Invalid Location");
			Assert.IsFalse(viewModel.OnCreateClicked.CanExecute(null));
		}

		[Test]
		public void OnDataReceived()
		{
			viewModel.OnCreateClicked.Execute(null);
			mockService.ReceiveData("JohnDoesEmptyApp");
			Assert.AreEqual("JohnDoesEmptyApp", viewModel.Name);
		}

		[Test]
		public void SelectedStarterKit()
		{
			viewModel.SelectedStarterKit = "EmptyApp";
			Assert.AreEqual("EmptyApp", viewModel.SelectedStarterKit);
		}
	}
}