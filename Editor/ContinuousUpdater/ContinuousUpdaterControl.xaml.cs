using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using Microsoft.Win32;

namespace DeltaEngine.Editor.ContinuousUpdater
{
	public partial class ContinuousUpdaterControl : EditorPluginView
	{
		public ContinuousUpdaterControl()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new ContinuousUpdaterViewModel();
		}

		private ContinuousUpdaterViewModel viewModel;

		private void StopUpdatingButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.StopUpdating();
		}

		private void Restart(object sender, RoutedEventArgs e)
		{
			viewModel.Restart();
		}

		private void OpenInVisualStudioButton(object sender, RoutedEventArgs e)
		{
			viewModel.OpenCurrentTestInVisualStudio();
		}

		private void SelectAssemblyFilePathButtonClick(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				DefaultExt = ".dll;.exe",
				Filter = "C# Project Assembly (.dll, .exe)|*.dll;*.exe",
				InitialDirectory = InitialDirectoryForBrowseDialog
			};
			if (dialog.ShowDialog().Equals(true))
				viewModel.SelectedProject = new Project(dialog.FileName);
		}

		private const string InitialDirectoryForBrowseDialog = "";

		public string ShortName
		{
			get { return "Continuous Updater"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Start.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Activate() {}

		public void Deactivate() {}

		public void Send(IList<string> arguments)
		{
			string path = arguments[1];
			string project = arguments[2];
			string test = arguments[3];
			Logger.Info("Path: " + path);
			Logger.Info("Project: " + project);
			Logger.Info("Test: " + test);
		}
	}
}