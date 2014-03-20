using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuildMessageViewModelTests
	{
		[Test]
		public void CheckDataForInfoMessage()
		{
			const string InfoText = "Just a note";
			AppBuildMessage infoMessage = AppBuilderTestExtensions.AsBuildTestInfo(InfoText);
			var infoMessageViewModel = new AppBuildMessageViewModel(infoMessage);
			Assert.IsTrue(infoMessageViewModel.ImagePath.Contains("Info"));
			AssertMessageTextAndProject(InfoText, infoMessageViewModel);
			Assert.IsEmpty(infoMessageViewModel.FileWithLineAndColumn);
			AssertMessageIsoData(infoMessageViewModel);
		}

		private static void AssertMessageTextAndProject(string expectedText,
			AppBuildMessageViewModel messageViewModel)
		{
			Assert.AreEqual(expectedText, messageViewModel.Message);
			Assert.IsNotEmpty(messageViewModel.Project);
		}

		private static void AssertMessageIsoData(AppBuildMessageViewModel messageViewModel)
		{
			Assert.IsNotEmpty(messageViewModel.IsoTime);
			Assert.IsFalse(messageViewModel.IsoTime.Contains("00:00:00"));
		}

		[Test]
		public void CheckDataForWarningMessage()
		{
			const string WarningText = "A bug but not crash";
			AppBuildMessage warningMessage = AppBuilderTestExtensions.AsBuildTestWarning(WarningText);
			var warningMessageViewModel = new AppBuildMessageViewModel(warningMessage);
			Assert.IsTrue(warningMessageViewModel.ImagePath.Contains("Warning"));
			AssertMessageTextAndProjectWithFileData(WarningText, warningMessageViewModel);
			AssertMessageIsoData(warningMessageViewModel);
		}

		private static void AssertMessageTextAndProjectWithFileData(string expectedText,
			AppBuildMessageViewModel messageViewModel)
		{
			AssertMessageTextAndProject(expectedText, messageViewModel);
			Assert.IsNotEmpty(messageViewModel.FileWithLineAndColumn);
		}

		[Test]
		public void CheckDataForErrorMessage()
		{
			const string ErrorText = "A bad crash";
			AppBuildMessage errorMessage = AppBuilderTestExtensions.AsBuildTestError(ErrorText);
			var errorMessageViewModel = new AppBuildMessageViewModel(errorMessage);
			Assert.IsTrue(errorMessageViewModel.ImagePath.Contains("Error"));
			AssertMessageTextAndProjectWithFileData(ErrorText, errorMessageViewModel);
			AssertMessageIsoData(errorMessageViewModel);
		}
	}
}
