using System.Runtime.InteropServices;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	public class DeviceTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawRedBackground()
		{
			Resolve<Window>().BackgroundColor = Color.Red;
		}

		[Test]
		public void SizeChanged()
		{
			Resolve<Window>().ViewportPixelSize = new Size(200, 100);
			Assert.AreEqual(new Size(200, 100), Resolve<Window>().ViewportPixelSize);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void SetFullscreenModeToJustWindowWithoutDeviceAndShowRedBackground()
		{
			var settings = Resolve<Settings>();
			settings.StartInFullscreen = true; // does not mean we really go fullscreen without device!
			Resolve<Window>().BackgroundColor = Color.Red;
			settings.StartInFullscreen = false;
		}

		[Test]
		public void SetFullscreenCallsDevicesOnFullscreenChanged()
		{
			var device = Resolve<Device>() as MockDevice;
			if (device == null)
				return; //ncrunch: no coverage
			Assert.IsFalse(device.OnFullscreenChangedCalled);
			var window = Resolve<Window>();
			window.SetFullscreen(Size.One);
			Assert.IsTrue(device.OnFullscreenChangedCalled);
		}

		[Test]
		public void ToggleFullscreenModeWithSpaceKey()
		{
			var window = Resolve<Window>();
			window.BackgroundColor = Color.Red;
			bool fullscreen = false;
			var screenSize = GetScreenSize();
			//ncrunch: no coverage start
			new Command(() =>
			{
				if (fullscreen)
					window.SetWindowed();
				else
					window.SetFullscreen(screenSize);
				fullscreen = !fullscreen;
			}).Add(new KeyTrigger(Key.Space));
			//ncrunch: no coverage end
		}

		private static Size GetScreenSize()
		{
			const int ScreenWidth = 0;
			const int ScreenHeight = 1;
			return new Size(GetSystemMetrics(ScreenWidth), GetSystemMetrics(ScreenHeight));
		}

		[DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
		private static extern int GetSystemMetrics(int systemMetric);

		[Test]
		public void OnSet3DModeActionIsCalledWhenSetting3DMode()
		{
			var device = Resolve<Device>() as MockDevice;
			if (device == null)
				return; //ncrunch: no coverage
			Assert.IsFalse(device.OnSet3DModeCalled);
			device.Set3DMode();
			Assert.IsTrue(device.OnSet3DModeCalled);
		}
	}
}