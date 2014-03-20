using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class OrientationExtensionsTests
	{
		[Test]
		public void IsLandscape()
		{
			Assert.IsTrue(Orientation.Landscape.IsLandscape());
			Assert.IsFalse(Orientation.Portrait.IsLandscape());
		}

		[Test]
		public void IsPortrait()
		{
			Assert.IsTrue(Orientation.Portrait.IsPortrait());
			Assert.IsFalse(Orientation.Landscape.IsPortrait());
		}
	}
}