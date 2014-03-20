using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Networking.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// ViewModel of the the whole AppBuilder which manages all available actions like building an app
	/// for a specific platform, listing of all possbile build issues of the app and also provides a
	/// listing of already built apps in past.
	/// </summary>
	public class AppBuilderViewModel : ViewModelBase
	{
		public AppBuilderViewModel(Service service)
		{
			Service = service;
			availableCodeProjectsWithContentProjects = new Dictionary<ProjectEntry, string>();
			MessagesListViewModel = new AppBuildMessagesListViewModel();
			AppListViewModel = new BuiltAppsListViewModel(Settings.Current);
			AppListViewModel.RebuildRequest += OnAppRebuildRequest;
			BuildCommand = new RelayCommand(OnBuildExecuted, () => IsBuildActionExecutable);
			HelpCommand = new RelayCommand(OpenAppBuilderFeaturesPage);
			GotoUserProfilePageCommand = new RelayCommand(OpenUserProfilePage);
			GotoBuiltAppsDirectoryCommand = new RelayCommand(OpenLocalBuiltAppsDirectory);
			service.AvailableProjectsChanged += RefreshListOfAvailableCodeProjects;
			service.SolutionFilePathOfContentProjectChanged += OnSolutionFilePathOfContentProjectChanged;
			RefreshListOfAvailableCodeProjects();
			service.ProjectChanged += AutoSelectNewCodeProject;
			AutoSelectNewCodeProject();
			service.DataReceived += OnServiceMessageReceived;
			Service.Send(new SupportedPlatformsRequest());
			SelectedPlatform = PlatformName.Windows;
			IsRebuildForced = false;
		}

		private readonly Dictionary<ProjectEntry, string> availableCodeProjectsWithContentProjects;
		public PlatformName[] SupportedPlatforms { get; private set; }
		public Service Service { get; private set; }
		public AppBuildMessagesListViewModel MessagesListViewModel { get; set; }
		public BuiltAppsListViewModel AppListViewModel { get; set; }
		public ICommand BuildCommand { get; private set; }
		public ICommand HelpCommand { get; private set; }
		public ICommand GotoUserProfilePageCommand { get; private set; }
		public ICommand GotoBuiltAppsDirectoryCommand { get; private set; }

		// ncrunch: no coverage start
		private void OnAppRebuildRequest(AppInfo app)
		{
			SendBuildRequestToServer(app.SolutionFilePath, app.Name, app.Platform, true);
		}
		// ncrunch: no coverage end

		private void SendBuildRequestToServer(string solutionFilePath,
			string projectNameInSolution, PlatformName platform, bool isRebuildOfCodeForced)
		{
			try
			{
				TrySendBuildRequestToServer(solutionFilePath, projectNameInSolution, platform,
					isRebuildOfCodeForced);
			}
			// ncrunch: no coverage start
			catch (Exception ex)
			{
				OnAppBuildMessageRecieved(GetErrorMessage(ex));
			}
			// ncrunch: no coverage end
		}

		private void TrySendBuildRequestToServer(string solutionFilePath,
			string projectNameInSolution, PlatformName platform, bool isRebuildOfCodeForced)
		{
			codeSolutionPathOfBuildingApp = UserSolutionPath;
			RaisePropertyChangedForIsBuildActionExecutable();
			SendBuildRequestToTheServer(solutionFilePath, projectNameInSolution, platform,
				isRebuildOfCodeForced);
		}

		private void RaisePropertyChangedForIsBuildActionExecutable()
		{
			RaisePropertyChanged("IsBuildActionExecutable");
		}

		private void SendBuildRequestToTheServer(string solutionFilePath, string projectNameInSolution,
			PlatformName platform, bool isRebuildOfCodeForced)
		{
			var projectData = new CodePacker(solutionFilePath, projectNameInSolution);
			var request = new AppBuildRequest(Path.GetFileName(solutionFilePath), projectNameInSolution,
				platform, projectData.GetPackedData());
			request.IsRebuildOfCodeForced = isRebuildOfCodeForced;
			request.ContentProjectName = GetContentProject(projectNameInSolution);
			Service.Send(request, false);
		}

		private string GetContentProject(string codeProjectName)
		{
			foreach (var codeProjectAndContentProject in availableCodeProjectsWithContentProjects)
				if (codeProjectAndContentProject.Key.Name == codeProjectName)
					return codeProjectAndContentProject.Value;
			throw new NoContentProjectAvailableForCodeProject(codeProjectName); // ncrunch: no coverage
		}

		// ncrunch: no coverage start
		public class NoContentProjectAvailableForCodeProject : Exception
		{
			public NoContentProjectAvailableForCodeProject(string codeProjectName)
				: base(codeProjectName) {}
		}
		// ncrunch: no coverage end

		public string UserSolutionPath
		{
			get { return userSolutionPath; }
			set
			{
				userSolutionPath = value;
				RaisePropertyChanged("UserSolutionPath");
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private string userSolutionPath;

		private string codeSolutionPathOfBuildingApp;

		private void RefreshListOfAvailableCodeProjects()
		{
			List<string> solutionList = GetFilePathListOfAvailableCodeSolutions();
			availableCodeProjectsWithContentProjects.Clear();
			foreach (string solutionFilePath in solutionList)
				AddCodeProjectsOfSolutionToAvailableProjects(solutionFilePath);
			RaisePropertyChanged("AvailableCodeProjects");
			AutoSelectNewCodeProject();
		}

		private void OnSolutionFilePathOfContentProjectChanged(string contentProjectName)
		{
			RefreshListOfAvailableCodeProjects();
		}

		private void AutoSelectNewCodeProject()
		{
			SelectedCodeProject = FindUserProjectInSolution();
		}

		private List<string> GetFilePathListOfAvailableCodeSolutions()
		{
			var solutionList = new List<string>();
			foreach (string contentProject in Service.AvailableProjects)
			{
				string solutionFilePath = Service.GetAbsoluteSolutionFilePath(contentProject);
				if (String.IsNullOrEmpty(solutionFilePath) || solutionList.Contains(solutionFilePath))
					continue;
				solutionList.Add(solutionFilePath);
				if (contentProject == "DeltaEngine.Tutorials")
					solutionList.Add(solutionFilePath.Replace("Basics", "Entities"));
			}
			return solutionList;
		}

		private void AddCodeProjectsOfSolutionToAvailableProjects(string solutionFilePath)
		{
			var solutionLoader = new SolutionFileLoader(solutionFilePath);
			var availableProjectsInSolution = solutionLoader.GetCSharpProjects();
			foreach (ProjectEntry projectEntry in availableProjectsInSolution)
			{
				string contentProject = GetContentProject(projectEntry);
				if (!String.IsNullOrEmpty(contentProject))
					if (!IsProjectAlreadyAddedToDictionary(projectEntry))
						availableCodeProjectsWithContentProjects.Add(projectEntry, contentProject);
			}
		}

		private string GetContentProject(ProjectEntry project)
		{
			if (project.Name.EndsWith(".Tests"))
				return null;
			string sameContentProject = Service.AvailableProjects.FirstOrDefault(c => c == project.Name);
			if (sameContentProject != null)
				return sameContentProject;
			return Service.AvailableProjects.FirstOrDefault(c => project.Name.StartsWith(c));
		}

		private bool IsProjectAlreadyAddedToDictionary(ProjectEntry projectEntry)
		{
			return availableCodeProjectsWithContentProjects.Keys.Any(p => p.Name == projectEntry.Name);
		}

		public ICollection<ProjectEntry> AvailableCodeProjects
		{
			get { return availableCodeProjectsWithContentProjects.Keys; }
		}

		private ProjectEntry FindUserProjectInSolution()
		{
			return AvailableCodeProjects.FirstOrDefault(
				csProject => csProject.Name.StartsWith(Service.ProjectName));
		}

		public ProjectEntry SelectedCodeProject
		{
			get { return selectedCodeProject; }
			set
			{
				if (value == null && IsSelectedProjectStillSelectable())
					return; // ncrunch: no coverage
				selectedCodeProject = value;
				RaisePropertyChanged("SelectedCodeProject");
				UpdateUserSolutionPathBasedOnSelectedCodeProject();
				DetermineEntryPointsOfProject();
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private bool IsSelectedProjectStillSelectable()
		{
			return selectedCodeProject != null &&
				AvailableCodeProjects.Contains(selectedCodeProject);
		}

		private ProjectEntry selectedCodeProject;

		private void UpdateUserSolutionPathBasedOnSelectedCodeProject()
		{
			if (SelectedCodeProject == null)
				return;
			string contentProjectName;
			if (availableCodeProjectsWithContentProjects.TryGetValue(SelectedCodeProject,
				out contentProjectName))
				UpdateUserSolutionPathFromService(contentProjectName);
			// We need custom logic here because Tutorials.Basics and Tutorials.Entities and share the
			// same content project
			if (SelectedCodeProject.Name.Contains("Tutorials.Entities"))
				UserSolutionPath = UserSolutionPath.Replace("Basics", "Entities");
		}

		private void UpdateUserSolutionPathFromService(string contentProjectName)
		{
			UserSolutionPath = Service.GetAbsoluteSolutionFilePath(contentProjectName);
		}

		private void DetermineEntryPointsOfProject()
		{
			AvailableEntryPointsInSelectedProject = new List<string>();
			if (SelectedCodeProject == null)
				return;
			AvailableEntryPointsInSelectedProject.Add(DefaultEntryPoint);
			RaisePropertyChanged("AvailableEntryPointsInSelectedProject");
			SelectedEntryPoint = DefaultEntryPoint;
			RaisePropertyChanged("SelectedEntryPoint");
		}

		public List<string> AvailableEntryPointsInSelectedProject { get; set; }
		private const string DefaultEntryPoint = "Program.Main";

		public string SelectedEntryPoint
		{
			get { return selectedEntryPoint; }
			set
			{
				selectedEntryPoint = value;
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private string selectedEntryPoint;

		public PlatformName SelectedPlatform
		{
			get { return selectedPlatform; }
			set
			{
				selectedPlatform = value;
				RaisePropertyChanged("SelectedPlatform");
				RaisePropertyChangedForIsBuildActionExecutable();
			}
		}

		private PlatformName selectedPlatform;

		// ncrunch: no coverage start
		private AppBuildMessage GetErrorMessage(Exception ex)
		{
			string errorMessage = "Failed to send BuildRequest to server because " + ex;
			return new AppBuildMessage(errorMessage)
			{
				Filename = Path.GetFileName(UserSolutionPath),
				Type = AppBuildMessageType.BuildError,
			};
		}
		// ncrunch: no coverage end

		protected void OnBuildExecuted()
		{
			Logger.Info("Build Request sent");
			MessagesListViewModel.ClearMessages();
			SendBuildRequestToServer(UserSolutionPath, SelectedCodeProject.Name, SelectedPlatform,
				IsRebuildForced);
		}

		// ncrunch: no coverage start
		private static void OpenAppBuilderFeaturesPage()
		{
			Process.Start("http://http://deltaengine.net/features/appbuilder");
		}

		private static void OpenUserProfilePage()
		{
			Process.Start("http://deltaengine.net/account/Projects/");
		}

		public void OpenLocalBuiltAppsDirectory()
		{
			Process.Start(AppListViewModel.AppStorageDirectory);
		}
		// ncrunch: no coverage end

		private void OnServiceMessageReceived(object serviceMessage)
		{
			if (serviceMessage is SupportedPlatformsResult)
				OnSupportedPlatformsResultRecieved((SupportedPlatformsResult)serviceMessage);
			if (serviceMessage is AppBuildMessage)
				OnAppBuildMessageRecieved((AppBuildMessage)serviceMessage);
			if (serviceMessage is AppBuildResult)
				OnAppBuildResultRecieved((AppBuildResult)serviceMessage);
			if (serviceMessage is AppBuildFailed)
				OnAppBuildFailedRecieved((AppBuildFailed)serviceMessage);
			if (serviceMessage is ServerError)
				OnServerError((ServerError)serviceMessage);
		}

		private void OnSupportedPlatformsResultRecieved(SupportedPlatformsResult platformsMessage)
		{
			SupportedPlatforms = platformsMessage.Platforms;
			RaisePropertyChanged("SupportedPlatforms");
			RaisePropertyChangedForIsBuildActionExecutable();
		}

		private void OnAppBuildMessageRecieved(AppBuildMessage receivedMessage)
		{
			if (receivedMessage.Type == AppBuildMessageType.BuildInfo)
			{
				Logger.Info(receivedMessage.Text);
				return;
			}
			Logger.Warning(receivedMessage.Text);
			MessagesListViewModel.AddMessage(receivedMessage);
			if (receivedMessage.Type == AppBuildMessageType.BuildError)
				AllowBuildingAppsAgain();
		}

		private void AllowBuildingAppsAgain()
		{
			codeSolutionPathOfBuildingApp = null;
			RaisePropertyChangedForIsBuildActionExecutable();
		}

		private void OnAppBuildResultRecieved(AppBuildResult receivedBuildResult)
		{
			AppInfo appInfo = AppInfoExtensions.CreateAppInfo(
				Path.Combine(AppListViewModel.AppStorageDirectory, receivedBuildResult.PackageFileName),
				receivedBuildResult.Platform, receivedBuildResult.PackageGuid, DateTime.Now);
			appInfo.SolutionFilePath = codeSolutionPathOfBuildingApp;
			AllowBuildingAppsAgain();
			TriggerBuiltAppRecieved(appInfo, receivedBuildResult.PackageFileData);
		}

		private void TriggerBuiltAppRecieved(AppInfo appInfo, byte[] appData)
		{
			if (BuiltAppRecieved != null)
				BuiltAppRecieved(appInfo, appData);
		}

		public event Action<AppInfo, byte[]> BuiltAppRecieved;

		private void OnAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			AllowBuildingAppsAgain();
			TriggerAppBuildFailedRecieved(buildFailedMessage);
		}

		private void TriggerAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			if (AppBuildFailedRecieved != null)
				AppBuildFailedRecieved(buildFailedMessage);
		}

		public event Action<AppBuildFailed> AppBuildFailedRecieved;

		private void OnServerError(ServerError serverError)
		{
			OnAppBuildFailedRecieved(new AppBuildFailed(serverError.Error));
		}

		public bool IsBuildActionExecutable
		{
			get
			{
				return IsCodeOfSelectedProjectAvailable && IsUserSelectedEntryPointValid &&
					IsSelectedPlatformValid && codeSolutionPathOfBuildingApp == null;
			}
		}

		private bool IsCodeOfSelectedProjectAvailable
		{
			get { return SelectedCodeProject != null; }
		}

		private bool IsUserSelectedEntryPointValid
		{
			get { return SelectedEntryPoint == DefaultEntryPoint; }
		}

		private bool IsSelectedPlatformValid
		{
			get
			{
				return SupportedPlatforms != null &&
					SupportedPlatforms.Any(supportedPlatform => SelectedPlatform == supportedPlatform);
			}
		}

		public bool IsRebuildForced { get; set; }
	}
}