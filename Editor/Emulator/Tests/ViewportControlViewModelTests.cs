using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class ViewportControlViewModelTests
	{
		[Test, RequiresSTA]
		public void CanCreateNewViewportControlViewModel()
		{
			var viewModel = new ViewportControlViewModel();
			Assert.IsNotNull(viewModel);
		}

		[Test, RequiresSTA]
		public void NewViewportControlViewModelShouldHaveFourTools()
		{
			var viewModel = new ViewportControlViewModel();
			Assert.AreEqual(4, viewModel.Tools.Count);
		}
	}
}