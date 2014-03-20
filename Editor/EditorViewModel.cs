using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Core.Properties;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Logging;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using DeltaEngine.Platforms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using Application = System.Windows.Application;

namespace DeltaEngine.Editor
{
	public class EditorViewModel : ViewModelBase
	{
		public EditorViewModel()
			: this(new EditorPluginLoader(), new FileSettings()) {}

		public EditorViewModel(EditorPluginLoader plugins, Settings settings)
		{
			this.plugins = plugins;
			Settings.Current = settings;
			service = new OnlineService();
			AvailableProjects = new List<ProjectNameAndFontWeight>();
			Error = Resources.GetApiKeyHere;
			SetupLogger();
			VersionNumber = new VersionNumber();
			plugins.FindAndLoadAllPlugins();
			RegisterCommands();
			SetApiKey(LoadDataFromRegistry("ApiKey"));
			SetInitialContentProject();
			ConnectToOnlineServiceAndTryToLogin();
			EditorPlugins = new List<EditorPluginView>();
			messageViewModel = new PopupMessageViewModel(service);
			messageViewModel.MessageUpdated += RaisePopupMessageProperties;
		}

		private readonly EditorPluginLoader plugins;
		private readonly OnlineService service;
		public List<ProjectNameAndFontWeight> AvailableProjects { get; private set; }

		public string Error
		{
			get { return error; }
			private set
			{
				error = value;
				RaisePropertyChanged("Error");
				RaisePropertyChanged("ErrorForegroundColor");
				RaisePropertyChanged("ErrorBackgroundColor");
			}
		}

		private string error;

		private void SetupLogger()
		{
			TextLogger = new TextLogger();
			TextLogger.NewLogMessage += () => RaisePropertyChanged("TextLogger");
			TextLogger.Write(Logger.MessageType.Info, "Welcome to the Delta Engine Editor");
		}

		public TextLogger TextLogger { get; private set; }

		public VersionNumber VersionNumber { get; private set; }

		private void RegisterCommands()
		{
			OnLoginButtonClicked = new RelayCommand(ValidateLogin);
			OnLogoutButtonClicked = new RelayCommand(Logout);
		}

		public ICommand OnLoginButtonClicked { get; private set; }
		public ICommand OnLogoutButtonClicked { get; private set; }

		private void SetApiKey(string apiKey)
		{
			ApiKey = apiKey;
			RaisePropertyChanged("ApiKey");
		}

		public string ApiKey { get; set; }

		private static string LoadDataFromRegistry(string name)
		{
			using (var key = Registry.CurrentUser.OpenSubKey(RegistryPathForEditorValues, false))
				if (key != null)
					return (string)key.GetValue(name);
			return null;
		}

		private const string RegistryPathForEditorValues = @"Software\DeltaEngine\Editor";

		private void SetInitialContentProject()
		{
			string project = LoadDataFromRegistry("SelectedProject");
			if (string.IsNullOrEmpty(project))
				project = DefaultContentProjectName;
			service.SetAvailableProjects(new[] { project });
			SelectedProject = project;
		}

		public string SelectedProject
		{
			get { return selectedProject; }
			set
			{
				if (isLoggedIn && selectedProject != value)
					service.ChangeProject(value);
				selectedProject = value;
				SaveCurrentProject();
				RaisePropertyChanged("SelectedProject");
			}
		}

		private string selectedProject;

		private void ConnectToOnlineServiceAndTryToLogin()
		{
			if (tryingToConnect)
				return;
			tryingToConnect = true;
			connection = new OnlineServiceConnection();
			connection.Connected += ValidateLogin;
			connection.Disconnected += ConnectionLost;
			connection.DataReceived += OnDataReceived;
			connection.Connect(Settings.Current.OnlineServiceIp, Settings.Current.OnlineServicePort,
				OnTimeout);
		}

		private bool tryingToConnect;
		private OnlineServiceConnection connection;

		private void OnTimeout()
		{
			Disconnect();
			Error = Settings.Current.OnlineServiceIp + ": " + Resources.ConnectionTimedOut;
		}

		private void Disconnect()
		{
			tryingToConnect = false;
			connection.Dispose();
			connection = new OnlineServiceConnection();
		}

		private void ValidateLogin()
		{
			if (!connection.IsConnected)
				ConnectToOnlineServiceAndTryToLogin();
			else if (!string.IsNullOrEmpty(ApiKey))
				connection.Send(new LoginRequest(ApiKey, "DeltaEngine.Editor"));
		}

		private void ConnectionLost()
		{
			if (!isLoggedIn || service.Viewport.Window.IsClosing)
				return;
			service.Viewport.Window.ShowMessageBox("Server connection lost",
				"Your connection to " + Settings.Current.OnlineServiceIp + ":" +
					Settings.Current.OnlineServicePort + " has been lost. The Editor window will close now.",
				new[] { "OK" });
			Application.Current.Dispatcher.Invoke(new Action(Application.Current.Shutdown));
		}

		private void OnDataReceived(object message)
		{
			var serverError = message as ServerError;
			var projectError = message as ProjectError;
			var projectNames = message as ProjectNamesResult;
			var loginMessage = message as LoginSuccessful;
			var newProject = message as SetProject;
			var contentReady = message as ContentReady;
			if (serverError != null)
				ProcessAndLogServerError(serverError);
			if (projectError != null)
				ProcessErrorAndSendChangeProjectRequest(projectError);
			else if (projectNames != null)
				RefreshAvailableProjects(projectNames);
			else if (loginMessage != null)
				Login(loginMessage);
			else if (newProject != null)
				VerifyProject(newProject);
			else if (contentReady != null)
				IsContentReady = true;
		}

		private void ProcessAndLogServerError(ServerError serverError)
		{
			Error = serverError.ToString();
			LogAndShowError();
		}

		private void LogAndShowError()
		{
			Logger.Warning(Error);
			service.Viewport.Window.ShowMessageBox("Server Reported Error", Error, new[] {"OK"});
		}

		private void ProcessErrorAndSendChangeProjectRequest(ProjectError projectError)
		{
			Error = projectError.ErrorMessage + ": " + projectError.ProjectName;
			LogAndShowError();
			selectedProject = DefaultContentProjectName;
			Logger.Info("Trying to change project to default project: " + selectedProject);
			connection.Send(new ChangeProjectRequest(selectedProject));
		}

		public const string DefaultContentProjectName = "GhostWars";

		private void RefreshAvailableProjects(ProjectNamesResult projectNames)
		{
			string[] allProjectNames =
				projectNames.PrivateProjects.Concat(projectNames.PublicProjects).ToArray();
			service.SetAvailableProjects(allProjectNames);
			AvailableProjects = new List<ProjectNameAndFontWeight>();
			var projectNamesAndWeight = new List<ProjectNameAndFontWeight>();
			foreach (var projectName in projectNames.PrivateProjects)
				projectNamesAndWeight.Add(new ProjectNameAndFontWeight(projectName, FontWeights.Bold));
			string tutorials = "";
			foreach (var projectName in projectNames.PublicProjects)
			{
				if (projectName == "DeltaEngine.Tutorials")
				{
					tutorials = projectName;
					continue;
				}
				projectNamesAndWeight.Add(new ProjectNameAndFontWeight(projectName, FontWeights.Normal));
			}
			AvailableProjects.AddRange(projectNamesAndWeight);
			if (!string.IsNullOrEmpty(tutorials))
				AvailableProjects.Add(new ProjectNameAndFontWeight(tutorials, FontWeights.Normal));
			RaisePropertyChanged("AvailableProjects");
		}

		private void Login(LoginSuccessful loginMessage)
		{
			connection.Send(new ProjectNamesRequest());
			Service.Connect(loginMessage.UserName, connection);
			AccountImage = loginMessage.AccountImagePath;
			SaveApiKey();
			SaveCurrentProject();
			IsLoggedIn = true;
			RaisePropertyChanged("AccountImage");
			RaisePropertyChanged("Service");
			Service.ChangeProject(SelectedProject);
		}

		public OnlineService Service
		{
			get { return service; }
		}

		public string AccountImage { get; private set; }

		public void SaveApiKey()
		{
			SaveDataInRegistry("ApiKey", ApiKey);
		}

		private static void SaveDataInRegistry(string name, string data)
		{
			if (data == null)
				return;
			using (var registryKey = Registry.CurrentUser.CreateSubKey(RegistryPathForEditorValues))
				if (registryKey != null)
					registryKey.SetValue(name, data);
		}

		private void SaveCurrentProject()
		{
			SaveDataInRegistry("SelectedProject", SelectedProject);
		}

		public bool IsLoggedIn
		{
			get { return isLoggedIn; }
			private set
			{
				isLoggedIn = value;
				RaisePropertyChanged("IsLoggedIn");
				RaisePropertyChanged("LoginPanelVisibility");
				RaisePropertyChanged("EditorPanelVisibility");
			}
		}

		private bool isLoggedIn;

		private void VerifyProject(SetProject newProject)
		{
			IsContentReady = false;
			DeleteProjectVisibility = newProject.Permissions == ProjectPermissions.Full
				? Visibility.Visible : Visibility.Collapsed;
			RaisePropertyChanged("DeleteProjectVisibility");
			if (newProject.Permissions != ProjectPermissions.None)
				return;
			selectedProject = DefaultContentProjectName;
			Logout();
		}

		public bool IsContentReady
		{
			get { return isContentReady; }
			set
			{
				isContentReady = value;
				RaisePropertyChanged("IsContentReady");
			}
		}

		private bool isContentReady;

		public Visibility DeleteProjectVisibility { get; private set; }

		private void Logout()
		{
			SetApiKey("");
			SaveApiKey();
			SaveCurrentProject();
			IsLoggedIn = false;
			Error = Resources.GetApiKeyHere;
			Disconnect();
		}

		public Brush ErrorForegroundColor
		{
			get { return GetErrorBrushColor().Item1; }
		}

		private Tuple<Brush, Brush> GetErrorBrushColor()
		{
			if (Error.Contains(Resources.ConnectionTimedOut))
				return new Tuple<Brush, Brush>(Brushes.White, Brushes.DarkRed);
			if (Error == Resources.GetApiKeyHere)
				return new Tuple<Brush, Brush>(Brushes.Blue, Brushes.Transparent);
			return new Tuple<Brush, Brush>(Brushes.Black, Brushes.Transparent);
		}

		public Brush ErrorBackgroundColor
		{
			get { return GetErrorBrushColor().Item2; }
		}

		public Visibility LoginPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Hidden : Visibility.Visible; }
		}

		public Visibility ErrorVisibility
		{
			get { return Error != "" ? Visibility.Visible : Visibility.Hidden; }
		}

		public Visibility EditorPanelVisibility
		{
			get { return IsLoggedIn ? Visibility.Visible : Visibility.Hidden; }
		}

		public List<EditorPluginView> EditorPlugins { get; private set; }

		private readonly PopupMessageViewModel messageViewModel;

		private void RaisePopupMessageProperties()
		{
			RaisePropertyChanged("PopupText");
			RaisePropertyChanged("PopupVisibility");
		}

		public string PopupText
		{
			get { return messageViewModel.Text; }
		}

		public Visibility PopupVisibility
		{
			get { return messageViewModel.Visiblity; }
		}

		public void AddAllPlugins()
		{
			foreach (var pluginType in plugins.UserControlsType)
				CreatePlugin(pluginType);
		}

		private void CreatePlugin(Type pluginType)
		{
			try
			{
				TryCreatePlugin(pluginType);
			}
			catch (MissingMethodException ex)
			{
				Logger.Error(new EditorPluginViewHasNoParameterlessConstructor(pluginType.ToString(), ex));
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private void TryCreatePlugin(Type pluginType)
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			var instance = Activator.CreateInstance(pluginType) as EditorPluginView;
			stopwatch.Stop();
			if (stopwatch.ElapsedMilliseconds > 50 && instance != null)
				Logger.Warning("Initialization of plugin " + instance.ShortName + " took too long: " +
					stopwatch.ElapsedMilliseconds + "ms");
			if (instance != null)
				InsertPluginAtRightPosition(instance);
		}

		private void InsertPluginAtRightPosition(EditorPluginView instance)
		{
			if (EditorPlugins.Contains(instance))
				return;
			if (instance.GetType().Name == "SampleBrowserView" ||
				instance.GetType().Name == "ProjectCreatorView")
				EditorPlugins.Insert(0, instance);
			else
				EditorPlugins.Add(instance);
		}

		private class EditorPluginViewHasNoParameterlessConstructor : Exception
		{
			public EditorPluginViewHasNoParameterlessConstructor(string plugin, Exception inner)
				: base("The Editor Plugin " + plugin + " is missing a parameterless constructor.", inner) {}
		}

		public bool StartEditorMaximized
		{
			get { return Settings.Current.StartInFullscreen; }
			set { Settings.Current.StartInFullscreen = value; }
		}

		public void ShowMessageBoxToDeleteCurrentProject()
		{
			bool dialogResult =
				service.Viewport.Window.ShowMessageBox(
					"Delete Content Project", "Are you sure you want to permanently delete " + service.ProjectName +
							" and all of its content?",new []{"Yes", "No"}) == "Yes";
			if (dialogResult)
				service.Send(new DeleteProject(service.ProjectName));
		}

		public Thickness ResizeBorderThickness
		{
			get { return new Thickness(maximizer != null && maximizer.isMaximized ? 0 : 5); }
		}

		internal MaximizerForEmptyWindows maximizer;

		public void UpdateBorderThicknessOfChromeStyle()
		{
			RaisePropertyChanged("ResizeBorderThickness");
		}
	}
}