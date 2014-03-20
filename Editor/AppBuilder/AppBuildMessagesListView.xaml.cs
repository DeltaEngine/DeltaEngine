using System.Windows;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// The list control which visualizes build messages (infos, errors, warnings) that can occur
	/// during the building process of an application via the BuildService.
	/// </summary>
	public partial class AppBuildMessagesListView
	{
		public AppBuildMessagesListView()
			: this(new AppBuildMessagesListViewModel()) {}

		public AppBuildMessagesListView(AppBuildMessagesListViewModel messagesViewModel)
		{
			InitializeComponent();
			ViewModel = messagesViewModel;
		}

		public AppBuildMessagesListViewModel ViewModel
		{
			get { return viewModel; }
			set
			{
				viewModel = value;
				DataContext = value;
			}
		}

		private AppBuildMessagesListViewModel viewModel;

		private void UpdateMessageColumnWidth(object sender, SizeChangedEventArgs e)
		{
			// We need to resize the grid columns ourselves as WPF does not support this feature (Auto
			// will just always resize to the content, which is not what we want). A better and more
			// complete solution can be found here:
			// http://www.codeproject.com/KB/grid/ListView_layout_manager.aspx
			// However, this is too much work to implement right now!
			double widthOfOtherColumns = IconGridViewColumn.Width + TimeGridViewColumn.Width +
				ProjectGridViewColumn.Width + FileGridViewColumn.Width;
			const double ColumnBorderCorrection = 10;
			double newMessageColumnWidth = (ActualWidth - ColumnBorderCorrection) - widthOfOtherColumns;
			if (newMessageColumnWidth < 50)
				newMessageColumnWidth = 50;
			MessageGridViewColumn.Width = newMessageColumnWidth;
		}

		private void OnCopyMessageClicked(object sender, RoutedEventArgs e)
		{
			var messageViewModel = OutputList.SelectedItem as AppBuildMessageViewModel;
			if (messageViewModel != null)
				ViewModel.CopyMessageToClipboard(messageViewModel);
		}
	}
}