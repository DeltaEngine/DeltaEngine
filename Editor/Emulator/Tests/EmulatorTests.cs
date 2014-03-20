using System.Windows;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class EmulatorTests
	{
		[Test]
		public void DefaultEmulatorHasNoImageAndNoScreenOffsetOrSize()
		{
			var defaultEmulator = new Emulator();
			Assert.AreEqual("", defaultEmulator.ImageResourceName);
			Assert.AreEqual(new Point(0, 0), defaultEmulator.ScreenPoint);
			Assert.AreEqual(new Size(0, 0), defaultEmulator.ScreenSize);
			Assert.AreEqual(Orientation.Landscape, defaultEmulator.Orientation);
			Assert.AreEqual("100%", defaultEmulator.Scale);
			Assert.IsTrue(defaultEmulator.IsDefault);
		}

		[Test]
		public void SpecificEmulatorIsNotDefault()
		{
			var emulator = new Emulator("ImageName", new Point(), new Size(), Orientation.Portrait,
				"100%");
			Assert.IsFalse(emulator.IsDefault);
		}
	}
}