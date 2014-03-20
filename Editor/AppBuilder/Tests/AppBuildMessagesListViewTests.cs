using System;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	public class AppBuildMessagesListViewTests
	{
		[Test, STAThread, Category("Slow")]
		public void VerifyViewWithMocking()
		{
			var listViewModel = CreateViewModelWithDummyMessages();
			WpfApprovals.Verify(CreateVerifiableWindowFromViewModel(listViewModel));
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(
			AppBuildMessagesListViewModel listViewModel)
		{
			return new WpfWindow
			{
				Content = new AppBuildMessagesListView(listViewModel),
				Width = 800,
				Height = 480
			};
		}

		private static AppBuildMessagesListViewModel CreateViewModelWithDummyMessages()
		{
			var listViewModel = new AppBuildMessagesListViewModel();
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestWarning("A simple build warning"));
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("A simple build error"));
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("A second simple build error"));
			return listViewModel;
		}

		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithWithDummyMessages()
		{
			var listViewModel = CreateViewModelWithDummyMessages();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("This error was added later"));
			window.ShowDialog();
		}
	}
}