using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.SpriteFontCreator
{
	/// <summary>
	/// Interaction logic for FontCreatorView.xaml
	/// </summary>
	public partial class FontCreatorView : EditorPluginView
	{
		public FontCreatorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = viewModel = new FontCreatorViewModel(service);
			service.ProjectChanged += ResetViewModel;
		}

		private FontCreatorViewModel viewModel;

		private void ResetViewModel()
		{
			Dispatcher.Invoke(new Action(viewModel.Reset));
		}

		public void Activate() {}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Font Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Font.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		private void WeakQualityCheckBoxClick(object sender, RoutedEventArgs e) {}
		private void WeakPlusButtonClick(object sender, RoutedEventArgs e) {}
		private void WeakMinusButtonClick(object sender, RoutedEventArgs e) {}
		private void UnicodeHelpButtonClick(object sender, RoutedEventArgs e) {}
		private void UnicodeExtendedButtonClick(object sender, RoutedEventArgs e) {}
		private void UnicodeEuropeanPlusButtonClick(object sender, RoutedEventArgs e) {}
		private void UnicodeEuropeanButtonClick(object sender, RoutedEventArgs e) {}
		private void TipsButtonClicked(object sender, RoutedEventArgs e) {}
		private void StyleChangedClick(object sender, RoutedEventArgs e) {}

		private void SaveButtonClicked(object sender, RoutedEventArgs e)
		{
			viewModel.GenerateFontFromSettings();
		}

		private void NormalQualityCheckBoxClick(object sender, RoutedEventArgs e) {}
		private void NormalPlusButtonClick(object sender, RoutedEventArgs e) {}
		private void NormalMinusButtonClick(object sender, RoutedEventArgs e) {}
		private void LegacyQualityCheckBoxClick(object sender, RoutedEventArgs e) {}
		private void LegacyPlusButtonClick(object sender, RoutedEventArgs e) {}
		private void LegacyMinusButtonClick(object sender, RoutedEventArgs e) {}

		private void ImportFontButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.OpenImportdialogue();
		}

		private void BestPlusButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.BestFontSize++;
		}

		private void BestMinusButtonClick(object sender, RoutedEventArgs e)
		{
			viewModel.BestFontSize--;
		}

		private void AsciiButtonClick(object sender, RoutedEventArgs e) {}
		private void AdvancedSettingsCheckBoxClick(object sender, RoutedEventArgs e) {}
		private void AddEuroButtonClick(object sender, RoutedEventArgs e) {}
		private void AddCyrillicButtonClick(object sender, RoutedEventArgs e) {}

		private void OnDropDownClosed(object sender, EventArgs e)
		{
			viewModel.SetControls();
		}
	}
}