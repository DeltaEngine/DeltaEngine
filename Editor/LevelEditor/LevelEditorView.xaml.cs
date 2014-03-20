using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.GameLogic;

namespace DeltaEngine.Editor.LevelEditor
{
	//ncrunch: no coverage start
	public partial class LevelEditorView : EditorPluginView
	{
		public LevelEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new LevelEditorViewModel(service);
			service.ProjectChanged += ResetViewModel;
			service.ContentUpdated +=
				(type, name) => Dispatcher.Invoke(new Action(() => viewModel.UpdateLists()));
			service.ContentDeleted +=
				name => Dispatcher.Invoke(new Action(() => viewModel.UpdateLists()));
		}

		private LevelEditorViewModel viewModel;

		private void ResetViewModel()
		{
			Dispatcher.Invoke(new Action(viewModel.ResetLevelEditor));
		}

		private void OpenXmlFileButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.xmlSaver.OpenXmlFile();
		}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			viewModel.SaveToServer();
		}

		private void AddWaveButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.AddWave();
		}

		private void RemoveWaveButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.RemoveSelectedWave();
		}

		private void WavesListBoxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			viewModel.SelectedWave = WavesListBox.SelectedItem as Wave;
		}

		public string ShortName
		{
			get { return "Level Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Level.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		public void Activate()
		{
			viewModel.ResetLevelEditor();
			viewModel.levelCommands.SetCommands();
			viewModel.cameraSliders.CreateSliders();
			viewModel.cameraSliders.Show();
		}

		public void Deactivate()
		{
			viewModel.ResetLevelEditor();
		}

		private void OnWaveNameDropDownClosed(object sender, EventArgs e)
		{
			viewModel.SetWaveProperties();
		}

		private void ClearLevel(object sender, RoutedEventArgs e)
		{
			viewModel.ClearLevel();
		}

		private void ClearWaves(object sender, RoutedEventArgs e)
		{
			viewModel.ClearWaves();
		}

		private void IncreaseBgImageSize(object sender, MouseButtonEventArgs e)
		{
			viewModel.IncreaseBgImageSize();
		}

		private void DecreaseBgImageSize(object sender, MouseButtonEventArgs e)
		{
			viewModel.DecreaseBgImageSize();
		}

		private void ResetBgImageSize(object sender, MouseButtonEventArgs e)
		{
			viewModel.ResetBgImageSize();
		}

		private void SetBackgroundToNone(object sender, MouseButtonEventArgs e)
		{
			if (viewModel.Is3D)
				viewModel.SelectedBackgroundModel = "";
			else
				viewModel.SelectedBackgroundImage = "";
		}

		private void OnLevelObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LevelObjectsList.SelectedItem != null)
				viewModel.SelectedLevelObject = LevelObjectsList.SelectedItem.ToString();
		}
	}
}