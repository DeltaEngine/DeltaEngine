using System.Windows;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using DeltaEngine.Editor.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	/// <summary>
	/// Tests for the visual representation of the project creation.
	/// </summary>
	[UseReporter(typeof(DiffReporter)), Category("Slow")]
	public class ProjectCreatorViewTests
	{
		[Test]
		public void InitialView()
		{
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(CreateViewModel()));
		}

		private static ProjectCreatorViewModel CreateViewModel()
		{
			return new ProjectCreatorViewModel(new MockService("John Doe", "LogoApp"));
		}

		private static Window CreateVerifiableWindowFromViewModel(ProjectCreatorViewModel viewModel)
		{
			viewModel.OnLocationChanged.Execute(@"C:\code\DeltaEngine\");
			return new Window { Content = new ProjectCreatorView(viewModel), Width = 600, Height = 300 };
		}

		[Test]
		public void ChangeName()
		{
			var viewModel = CreateViewModel();
			viewModel.OnNameChanged.Execute("ChangedProjectName");
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void ChangeFrameworkSelectionToSharpDX()
		{
			var viewModel = CreateViewModel();
			viewModel.OnFrameworkSelectionChanged.Execute(1);
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void ChangeFrameworkSelectionToSlimDX()
		{
			var viewModel = CreateViewModel();
			viewModel.OnFrameworkSelectionChanged.Execute(2);
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void ChangeFrameworkSelectionToXna()
		{
			var viewModel = CreateViewModel();
			viewModel.OnFrameworkSelectionChanged.Execute(3);
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void ChangePath()
		{
			var viewModel = CreateViewModel();
			viewModel.OnLocationChanged.Execute("C:\\DeltaEngine\\");
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void CannotCreateWithInvalidProjectName()
		{
			var viewModel = CreateViewModel();
			viewModel.OnNameChanged.Execute("name");
			viewModel.OnCreateClicked.Execute(null);
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}

		[Test]
		public void CannotCreateWithInvalidProjectLocation()
		{
			var viewModel = CreateViewModel();
			viewModel.OnLocationChanged.Execute("Delta Engine");
			viewModel.OnCreateClicked.Execute(null);
			WpfApprovals.Verify(() => CreateVerifiableWindowFromViewModel(viewModel));
		}
	}
}