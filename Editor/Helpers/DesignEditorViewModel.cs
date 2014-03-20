using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Logging;

namespace DeltaEngine.Editor.Helpers
{
	/// <summary>
	/// Design-time DataContext of EditorViewModel for EditorView Designer
	/// </summary>
	public class DesignEditorViewModel
	{
		//ncrunch: no coverage start
		public DesignEditorViewModel()
		{
			SetLoginState(true);
			SetErrorMessage("Error message");
			LogInfoToLogOutput("Log message");
			ApiKey = "1A2B3C4D-5E6F-7G8H-9I0J-1K2L3M4N5O6P";
			Service = new MockService("exDreamDuck", "Game - The Game");
			AvailableProjects = new List<string> { "Project One", "Project Two", "Project Three" };
			SelectedProject = AvailableProjects[0];
			EditorPlugins = new List<EditorPluginView> { new DesignEditorPlugin() };
			IsContentReady = false;
			OnLoginButtonClicked = OnLogoutButtonClicked = null;
			VersionNumber = "v1.2.3";
			SetUpPopupMessage();
		}

		private void SetLoginState(bool isLoggedIn)
		{
			IsLoggedIn = isLoggedIn;
			LoginPanelVisibility = IsLoggedIn ? Visibility.Hidden : Visibility.Visible;
			EditorPanelVisibility = IsLoggedIn ? Visibility.Visible : Visibility.Hidden;
		}

		public bool IsLoggedIn { get; private set; }
		public Visibility LoginPanelVisibility { get; private set; }
		public Visibility EditorPanelVisibility { get; private set; }

		private void SetErrorMessage(string errorMessage)
		{
			Error = errorMessage;
			ErrorVisibility = string.IsNullOrEmpty(Error) ? Visibility.Hidden : Visibility.Visible;
			ErrorForegroundColor = string.IsNullOrEmpty(Error) ? Brushes.Black : Brushes.White;
			ErrorBackgroundColor = string.IsNullOrEmpty(Error) ? Brushes.Transparent : Brushes.DarkRed;
		}

		public string Error { get; private set; }
		public Visibility ErrorVisibility { get; private set; }
		public Brush ErrorForegroundColor { get; private set; }
		public Brush ErrorBackgroundColor { get; private set; }

		private void LogInfoToLogOutput(string logMessage)
		{
			try
			{
				TryLogInfoToLogOutput(logMessage);
			}
			catch (Logger.LoggerWasAlreadyAttached ex)
			{
				Logger.Warning(ex);
			}
		}

		private void TryLogInfoToLogOutput(string logMessage)
		{
			TextLogger = new TextLogger();
			TextLogger.Write(Logger.MessageType.Info, logMessage);
		}

		public TextLogger TextLogger { get; private set; }

		public string ApiKey { get; set; }
		public MockService Service { get; private set; }
		public List<string> AvailableProjects { get; private set; }
		public string SelectedProject { get; set; }
		public List<EditorPluginView> EditorPlugins { get; private set; }
		public bool IsContentReady { get; private set; }
		public ICommand OnLoginButtonClicked { get; private set; }
		public ICommand OnLogoutButtonClicked { get; private set; }
		public string VersionNumber { get; private set; }

		private void SetUpPopupMessage()
		{
			PopupText = "Content updated";
			PopupVisibility = Visibility.Visible;
		}

		public string PopupText { get; private set; }
		public Visibility PopupVisibility { get; private set; }

		public Visibility DeleteProjectVisibility
		{
			get { return Visibility.Visible; }
		}

		public string AccountImage
		{
			get { return "Images/AppBuilder/BuildInfoIcon.png"; }
		}

		public Thickness ResizeBorderThickness
		{
			get { return new Thickness(5); }
		}
	}
}