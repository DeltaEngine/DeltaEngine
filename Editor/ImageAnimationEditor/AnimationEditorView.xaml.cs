using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	/// <summary>
	/// Interaction logic for AnimationEditorView.xaml
	/// </summary>
	public partial class AnimationEditorView : EditorPluginView
	{
		//ncrunch: no coverage start
		public AnimationEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			viewModel = new AnimationEditorViewModel(service);
			DataContext = viewModel;
			service.ProjectChanged += ChangeProject;
			service.ContentUpdated +=
				(t, s) => Dispatcher.Invoke(new Action(() => viewModel.RefreshOnContentChange()));
			service.ContentDeleted +=
				s => Dispatcher.Invoke(new Action(viewModel.RefreshOnContentChange));
		}

		private AnimationEditorViewModel viewModel;

		private void ChangeProject()
		{
			Dispatcher.Invoke(new Action(viewModel.ResetOnProjectChange));
		}

		public void Activate()
		{
			viewModel.ActivateAnimation();
		}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Image Animation"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ImageAnimation.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		private void RemoveSelectedImage(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.SelectedItem == null)
				return;
			Messenger.Default.Send(ImageViewList.SelectedIndex, "RemoveImage");
		}

		private void MoveImageUp(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.SelectedItem == null)
				return;
			Messenger.Default.Send(ImageViewList.SelectedIndex, "MoveImageUp");
		}

		private void MoveImageDown(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.SelectedItem == null)
				return;
			Messenger.Default.Send(ImageViewList.SelectedIndex, "MoveImageDown");
		}

		private void SaveAnimation(object sender, RoutedEventArgs e)
		{
			if (ImageViewList.Items.Count == 0)
				return;
			Messenger.Default.Send("SaveAnimation", "SaveAnimation");
		}

		private void AddImage(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("AddImage", "AddImage");
		}
	}
}