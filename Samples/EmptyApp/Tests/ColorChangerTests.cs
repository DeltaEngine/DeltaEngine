using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace EmptyApp.Tests
{
	public class ColorChangerTests : TestWithMocksOrVisually
	{
		[Test]
		public void CheckIfBackgroundColorWasChangedAfterOneSecond()
		{
			var app = new ColorChanger(Resolve<Window>());
			var initialColor = new Color();
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreNotEqual(initialColor, app.NextColor);
		}
	}
}