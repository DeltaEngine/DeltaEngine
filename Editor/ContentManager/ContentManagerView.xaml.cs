using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class ContentManagerView : EditorPluginView
	{
		//ncrunch: no coverage start
		public ContentManagerView()
		{
			InitializeComponent();
			Messenger.Default.Register<string>(this, "OpenEditorPlugin", OpenEditorPlugin);
		}

		private void OpenEditorPlugin(string name)
		{
			if (contentManagerViewModel != null)
				contentManagerViewModel.AddTypeToFilter(name);
			foreach (ToggleButton filterButton in FilterList.Children)
				filterButton.IsChecked = false;
			foreach (ToggleButton filterButton in FilterList.Children)
				if (filterButton.Name == name)
					filterButton.IsChecked = true;
		}

		public void Init(Service service)
		{
			DataContext = contentManagerViewModel = new ContentManagerViewModel(service);
			service.ContentUpdated += (type, name) =>
			{
				AddContentToContentList(type, name);
				ContentLoader.RemoveResource(name);
				if (isContentReadyForUse)
					Dispatcher.Invoke(
						new Action(
							() =>
								contentManagerViewModel.SelectedContent =
									new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(type), name)));
			};
			service.ContentDeleted += name =>
			{
				DeleteContentFromContentList(name);
				ContentLoader.RemoveResource(name);
			};
			service.ProjectChanged += () =>
			{
				isContentReadyForUse = false;
				RefreshContentList();
			};
			service.ContentReady += () =>
			{
				Dispatcher.Invoke(new Action(contentManagerViewModel.ShowStartContent));
				isContentReadyForUse = true;
			};
			contentManagerViewModel.RefreshContentList();
		}

		private bool isContentReadyForUse;

		public void Activate()
		{
			contentManagerViewModel.Activate();
		}

		public void Deactivate()
		{
			contentManagerViewModel.Deactivate();
		}

		private ContentManagerViewModel contentManagerViewModel;

		private void AddContentToContentList(ContentType type, string name)
		{
			Dispatcher.Invoke(
				new Action(() => contentManagerViewModel.AddContentToContentList(type, name)));
		}

		private void DeleteContentFromContentList(string name)
		{
			Dispatcher.Invoke(
				new Action(() => contentManagerViewModel.DeleteContentFromContentList(name)));
		}

		private void RefreshContentList()
		{
			Dispatcher.Invoke(new Action(contentManagerViewModel.RefreshContentList));
		}

		private void DeleteSelectedItems(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteContent", "DeleteContent");
		}

		public string ShortName
		{
			get { return "Content Manager"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Content.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		private void ChangeSelectedItem(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			if (e.NewValue == null)
				return;
			contentManagerViewModel.SelectedContent = e.NewValue;
		}

		private void OpenFileExplorer(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("OpenFileExplorerToAddNewContent", "OpenFileExplorerToAddNewContent");
		}

		private void DeleteContent(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("DeleteContent", "DeleteContent");
		}

		private void OnHelp(object sender, RoutedEventArgs e)
		{
			Process.Start("http://deltaengine.net/features/editor");
		}

		private void CheckContentType(object sender, RoutedEventArgs e)
		{
			if (contentManagerViewModel != null)
				contentManagerViewModel.AddTypeToFilter(((ToggleButton)sender).Name);
		}

		private void UnCheckContentType(object sender, RoutedEventArgs e)
		{
			if (contentManagerViewModel != null)
				contentManagerViewModel.RemoveTypeFromFilter(((ToggleButton)sender).Name);
		}

		private void DeleteContentWithSubContent(object sender, RoutedEventArgs e)
		{
			contentManagerViewModel.DeleteContentWithSubContent();
		}

		private void CopySelectedContent(object sender, RoutedEventArgs e)
		{
		}

		private void PasteCopiedContentIfPossible(object sender, RoutedEventArgs e)
		{
		}
	}
}