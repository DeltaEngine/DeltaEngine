using DeltaEngine.Platforms.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	[Category("Slow")]
	public class MouseDeviceCounterTests
	{
		//ncrunch: no coverage start
		[Test]
		public void GetNumberOfAvailableMice()
		{
			Assert.DoesNotThrow(delegate
			{
				var counter = new MouseDeviceCounter();
				Assert.Greater(counter.GetNumberOfAvailableMice(), 0);
			});
		}

		[Test]
		public void DeviceCounterInMouseProperty()
		{
			var windowsMouse = GetMouse();
			Assert.IsTrue(windowsMouse.IsAvailable);
		}

		private static WindowsMouse GetMouse()
		{
			var resolver = new MockResolver();
			return new WindowsMouse(resolver.Window);
		}
	}
}