using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.SpriteFontCreator.Tests
{
	public class FontCreatorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetupViewModel()
		{
			mockService = new MockService("TestName", "TestProject");
			viewModel = new FontCreatorViewModel(mockService);
		}

		private FontCreatorViewModel viewModel;
		private MockService mockService;

		[Test]
		public void StartFontCreationFromFirstSystemFont()
		{
			viewModel.UpdateAvailableDefaultFonts();
			viewModel.FamilyFilename = viewModel.AvailableDefaultFontNames[0];
			viewModel.ContentName = "TestFont";
			viewModel.GenerateFontFromSettings();
		}

		[Test]
		public void GetSystemFontsForSelection()
		{
			viewModel.UpdateAvailableDefaultFonts();
			Assert.IsNotEmpty(viewModel.AvailableDefaultFontNames);
		}

		[Test]
		public void NoFontCreationWithoutContentName()
		{
			viewModel.ContentName = "";
			viewModel.GenerateFontFromSettings();
		}

		[Test]
		public void SettingFontStyles()
		{
			SetStyles();
			Assert.IsTrue(viewModel.Bold && !viewModel.Italic && viewModel.Underline && 
				viewModel.AddShadow && viewModel.AddOutline);
		}

		private void SetStyles()
		{
			viewModel.AddShadow = false;
			viewModel.Bold = false;
			viewModel.Underline = false;
			viewModel.Italic = true;
			viewModel.Bold = true;
			viewModel.Italic = false;
			viewModel.Underline = true;
			viewModel.AddShadow = true;
			viewModel.AddOutline = false;
			viewModel.AddOutline = true;
		}
	}
}