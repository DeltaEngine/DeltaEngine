using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// The list control which shows the list of already built apps or the warnings and errors of the
	/// app which will be currently build.
	/// </summary>
	public partial class AppBuilderInfoListView
	{
		public AppBuilderInfoListView()
		{
			InitializeComponent();
			AppsLabel.MouseEnter += (sender, args) => ShowBorderForAppsListPanel();
			AppsLabel.MouseLeave += (sender, args) => HideBorderForAppsListPanel();
			ErrorsLabel.MouseEnter += (sender, args) => ShowBorderForErrorsPanel();
			ErrorsLabel.MouseLeave += (sender, args) => HideBorderForErrorsPanel();
			WarningsLabel.MouseEnter += (sender, args) => ShowBorderForWarningsPanel();
			WarningsLabel.MouseLeave += (sender, args) => HideBorderForWarningsPanel();
		}

		private void ShowBorderForAppsListPanel()
		{
			ShowBorder(AppsListPanelBorder);
		}

		private static void ShowBorder(Border border)
		{
			border.BorderBrush = new SolidColorBrush(Colors.DodgerBlue);
		}

		private void HideBorderForAppsListPanel()
		{
			if (!IsBuiltAppsListFocused)
				HideBorder(AppsListPanelBorder);
		}

		private static void HideBorder(Border border)
		{
			border.BorderBrush = new SolidColorBrush(Colors.Transparent);
		}

		private void ShowBorderForErrorsPanel()
		{
			ShowBorder(ErrorsPanelBorder);
		}

		private void HideBorderForErrorsPanel()
		{
			if (!MessagesViewModel.IsShowingErrorsAllowed)
				HideBorder(ErrorsPanelBorder);
		}

		private void ShowBorderForWarningsPanel()
		{
			ShowBorder(WarningsPanelBorder);
		}

		private void HideBorderForWarningsPanel()
		{
			if (!MessagesViewModel.IsShowingWarningsAllowed)
				HideBorder(WarningsPanelBorder);
		}

		public BuiltAppsListViewModel AppListViewModel
		{
			get { return BuiltAppsList.ViewModel; }
			set
			{
				BuiltAppsList.ViewModel = value;
				AppsLabel.DataContext = value;
				FocusBuiltAppsList();
			}
		}

		public void FocusBuiltAppsList()
		{
			MessagesViewModel.IsShowingErrorsAllowed = false;
			MessagesViewModel.IsShowingWarningsAllowed = false;
			BuildMessagesList.Visibility = Visibility.Collapsed;
			BuiltAppsList.Visibility = Visibility.Visible;
		}

		public AppBuildMessagesListViewModel MessagesViewModel
		{
			get { return BuildMessagesList.ViewModel; }
			set
			{
				if (BuildMessagesList.ViewModel != null)
					BuildMessagesList.ViewModel.PropertyChanged -= OnMessagesListPropertyChanged;
				BuildMessagesList.ViewModel = value;
				BuildMessagesList.ViewModel.PropertyChanged += OnMessagesListPropertyChanged;
				ErrorsLabel.DataContext = value;
				WarningsLabel.DataContext = value;
				FocusBuildMessagesList();
			}
		}

		private void OnMessagesListPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsShowingErrorsAllowed")
				UpdateErrorsStackPanelBackground();
			if (e.PropertyName == "IsShowingWarningsAllowed")
				UpdateWarningsStackPanelBackground();
		}

		private void UpdateErrorsStackPanelBackground()
		{
			if (MessagesViewModel.IsShowingErrorsAllowed)
				ShowBorderForErrorsPanel();
			else
				HideBorderForErrorsPanel();
		}

		private void UpdateWarningsStackPanelBackground()
		{
			if (MessagesViewModel.IsShowingWarningsAllowed)
				ShowBorderForWarningsPanel();
			else
				HideBorderForWarningsPanel();
		}

		private void OnErrorsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				MessagesViewModel.IsShowingErrorsAllowed = !MessagesViewModel.IsShowingErrorsAllowed;
			else
				FocusBuildMessagesList();
		}

		public bool IsBuildMessagesListFocused
		{
			get { return BuildMessagesList.Visibility == Visibility.Visible; }
		}

		public void FocusBuildMessagesList()
		{
			BuiltAppsList.Visibility = Visibility.Collapsed;
			BuildMessagesList.Visibility = Visibility.Visible;
			MessagesViewModel.IsShowingErrorsAllowed = true;
			HideBorderForAppsListPanel();
		}

		private void OnWarningsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuildMessagesListFocused)
				MessagesViewModel.IsShowingWarningsAllowed = !MessagesViewModel.IsShowingWarningsAllowed;
			else
				FocusBuildMessagesList();
		}

		private void OnPlatformsStackPanelClicked(object sender, MouseButtonEventArgs e)
		{
			if (IsBuiltAppsListFocused)
				return;

			FocusBuiltAppsList();
		}

		public bool IsBuiltAppsListFocused
		{
			get { return BuiltAppsList.Visibility == Visibility.Visible; }
		}
	}
}