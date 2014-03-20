using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class EmulatorViewModelTests
	{
		[SetUp]
		public void CreateEmulatorViewModel()
		{
			viewModel = new MockEmulatorViewModel();
		}

		private MockEmulatorViewModel viewModel;

		[Test]
		public void CheckDefaultValues()
		{
			Assert.AreEqual(2, viewModel.AvailableDevices.Count);
			Assert.AreEqual(viewModel.AvailableDevices[0], viewModel.SelectedDevice);
			Assert.AreEqual(2, viewModel.AvailableOrientations.Count);
			Assert.AreEqual(Orientation.Landscape, viewModel.SelectedOrientation);
			Assert.AreEqual(3, viewModel.AvailableScales.Count);
			Assert.AreEqual(viewModel.AvailableScales[2], viewModel.SelectedScale);
		}

		[Test]
		public void ChangeEmulator()
		{
			bool hasEmulatorChanged = false;
			viewModel.EmulatorChanged += name => hasEmulatorChanged = true;
			viewModel.ChangeEmulator();
			Assert.IsTrue(hasEmulatorChanged);
			Assert.AreEqual(viewModel.AvailableDevices[1], viewModel.SelectedDevice);
			Assert.AreEqual(Orientation.Portrait, viewModel.SelectedOrientation);
			Assert.AreEqual("75%", viewModel.SelectedScale);
		}
	}
}