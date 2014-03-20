using System;
using System.IO;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuilderViewModelTests
	{
		[SetUp]
		public void LoadAppBuilderViewModel()
		{
			CreateNewAppBuilderViewModel();
		}

		private void CreateNewAppBuilderViewModel()
		{
			service = new MockBuildService();
			viewModel = new AppBuilderViewModel(service);
			service.SetAvailableProjects(StartupProject, "Blocks", "GhostWars", TutorialsProject);
			service.ChangeProject(StartupProject);
		}

		private MockBuildService service;
		private AppBuilderViewModel viewModel;
		private const string StartupProject = "LogoApp";
		private const string TutorialsProject = "DeltaEngine.Tutorials";

		[Test]
		public void CheckSupportedAndSelectedPlatform()
		{
			Assert.IsNotEmpty(viewModel.SupportedPlatforms);
			foreach (PlatformName platform in viewModel.SupportedPlatforms)
				Logger.Info("SupportedPlatform: " + platform);
			Assert.AreNotEqual((PlatformName)0, viewModel.SelectedPlatform);
		}
		
		[Test]
		public void ThereShouldBeAlwaysAStartupSolution()
		{
			Assert.IsTrue(File.Exists(viewModel.UserSolutionPath), viewModel.UserSolutionPath);
		}

		[Test]
		public void CheckAvailableProjectsOfSamplesSolutionInEngineCodeDirectory()
		{
			string originalDirectory = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = PathExtensions.GetFallbackEngineSourceCodeDirectory();
				CreateNewAppBuilderViewModel();
				AssertSelectedSolutionProject();
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private void AssertSelectedSolutionProject()
		{
			Assert.AreEqual(viewModel.SelectedCodeProject.Name, service.ProjectName);
		}

		[Test]
		public void CheckAvailableProjectsOfSamplesSolutionInEngineInstallerDirectory()
		{
			const string EnginePathVariableName = PathExtensions.EnginePathEnvironmentVariableName;
			string originalDirectory = Environment.GetEnvironmentVariable(EnginePathVariableName);
			try
			{
				Environment.SetEnvironmentVariable(EnginePathVariableName, null);
				CreateNewAppBuilderViewModel();
				AssertSelectedSolutionProject();
			}
			finally
			{
				Environment.SetEnvironmentVariable(EnginePathVariableName, originalDirectory);
			}
		}

		[Test]
		public void CheckAvailableEntryPoints()
		{
			Assert.IsNotEmpty(viewModel.AvailableEntryPointsInSelectedProject);
			Assert.IsNotEmpty(viewModel.SelectedEntryPoint);
			Assert.Contains(viewModel.SelectedEntryPoint,
				viewModel.AvailableEntryPointsInSelectedProject);
		}

		[Test]
		public void SelectedProjectMustBeUpdatedWhenContentProjectHasChanged()
		{
			AssertSelectedProjectIsBuildable();
			service.ChangeProject("LogoApp");
			AssertSelectedProjectIsBuildable();
			service.ChangeProject("GhostWars");
			AssertSelectedProjectIsBuildable();
		}

		private void AssertSelectedProjectIsBuildable()
		{
			AssertSelectedSolutionProject();
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
		}

		[Test]
		public void ExcuteBuildCommand()
		{
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
			Assert.IsTrue(viewModel.BuildCommand.CanExecute(null));
			viewModel.BuiltAppRecieved += (app, data) => Logger.Info("Built app received: " + app.Name);
			viewModel.BuildCommand.Execute(null);
		}

		[Test]
		public void AppBuilderShouldBeAbleToHandleBuildMessage()
		{
			int numberOfWarngins = viewModel.MessagesListViewModel.Warnings.Count;
			service.RaiseAppBuildInfo("You shouldn't see this message");
			service.RaiseAppBuildWarning("An other build warning");
			Assert.AreEqual(numberOfWarngins + 1, viewModel.MessagesListViewModel.Warnings.Count);
		}

		[Test]
		public void AppBuilderShouldBeAbleToHandleFailedBuild()
		{
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
			viewModel.AppBuildFailedRecieved += (fail) => Logger.Info("Built failed: " + fail.Reason);
			service.ReceiveAppBuildFailed("Test harder, code better ^^");
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
		}

		[Test]
		public void ProjectsWhichShareOneContentProjectAreAllowedIfTheyShareSameNamespace()
		{
			service.ChangeProject("DeltaEngine.Tutorials");
			string basicsTutorialSolutionFilePath =
				Path.Combine(PathExtensions.GetFallbackEngineSourceCodeDirectory(), "Tutorials",
					"DeltaEngine.Tutorials.Basics.sln");
			viewModel.UserSolutionPath = basicsTutorialSolutionFilePath;
			Assert.IsTrue(viewModel.IsBuildActionExecutable);
		}

		[Test]
		public void NotifyTheServiceAboutChangedCodeSolutionPathWhenBuildIsExecuted()
		{
			const string CustomCodeSolutionFilePath = @"C:\Sample\MySampleGame.sln";
			viewModel.Service.CurrentContentProjectSolutionFilePath = CustomCodeSolutionFilePath;
			Assert.IsTrue(viewModel.BuildCommand.CanExecute(null));
			viewModel.BuildCommand.Execute(null);
			Assert.AreEqual(viewModel.UserSolutionPath, service.CurrentContentProjectSolutionFilePath);
		}

		[Test]
		public void ChangingTheSelectedCodeProjectWillUpdateSolutionFilePath()
		{
			string originalSolutionFilePath = viewModel.UserSolutionPath;
			ProjectEntry tutorialsProject = GetFirstEntitiesTutorialProjectEntry();
			Assert.IsNotNull(tutorialsProject);
			viewModel.SelectedCodeProject = tutorialsProject;
			Assert.AreNotEqual(originalSolutionFilePath, viewModel.UserSolutionPath);
			Assert.IsTrue(viewModel.BuildCommand.CanExecute(null));
			viewModel.BuildCommand.Execute(null);
		}

		private ProjectEntry GetFirstEntitiesTutorialProjectEntry()
		{
			return
				viewModel.AvailableCodeProjects.FirstOrDefault(
					projectEntry => projectEntry.Name.StartsWith("DeltaEngine.Tutorials.Entities"));
		}

		[Test]
		public void ReceivingServerErrorWhileBuildingWillAbortBuildProcess()
		{
			bool isAppBuildFailedTriggeredByServerError = false;
			viewModel.AppBuildFailedRecieved += failed =>  isAppBuildFailedTriggeredByServerError = true;
			service.ReceiveData(new ServerError("I have seen an error"));
			Assert.IsTrue(isAppBuildFailedTriggeredByServerError);
		}

		// ncrunch: no coverage start
		[Test, Category("Slow")]
		public void ExcuteHelpCommand()
		{
			Assert.IsTrue(viewModel.HelpCommand.CanExecute(null));
			viewModel.HelpCommand.Execute(null);
		}

		[Test, Category("Slow")]
		public void ExcuteGotoUserProfilePageCommand()
		{
			Assert.IsTrue(viewModel.GotoUserProfilePageCommand.CanExecute(null));
			viewModel.GotoUserProfilePageCommand.Execute(null);
		}

		[Test, Category("Slow")]
		public void ExcuteGotoBuiltAppsDirectoryCommand()
		{
			Assert.IsTrue(viewModel.GotoBuiltAppsDirectoryCommand.CanExecute(null));
			viewModel.GotoBuiltAppsDirectoryCommand.Execute(null);
		}

		[Test, Category("Slow"), Timeout(10000)]
		public void RequestRequild()
		{
			int numberOfRequests = service.NumberOfBuildRequests;
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			app.SolutionFilePath = SolutionExtensions.GetSamplesSolutionFilePath();
			viewModel.AppListViewModel.RequestRebuild(app);
			Assert.AreEqual(numberOfRequests + 1, service.NumberOfBuildRequests);
		}
	}
}