using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class DeviceTests
	{
		[TestFixtureSetUp]
		public void InitializeDevice()
		{
			device = new MockDevice();
		}

		private MockDevice device;

		internal class MockDevice : Device
		{
			public MockDevice()
			{
				Name = "No emulator or connected device available for this platform.";
				IsEmulator = false;
			}

			public override bool IsAppInstalled(AppInfo app)
			{
				return app != null && app.GetType() != typeof(NonExistingApp);
			}

			protected override void InstallApp(AppInfo app) { }
			protected override void UninstallApp(AppInfo app) { }
			protected override void LaunchApp(AppInfo app) {}
		}

		private class NonExistingApp : AppInfoTests.AppInfoWithMockDevice
		{
			public NonExistingApp()
				: base(PlatformName.Web, "NonExistingApp") { }
		}

		[Test]
		public void DeviceNameIsNotEmpty()
		{
			Assert.IsNotEmpty(device.Name);
		}

		[Test]
		public void NonExistingAppCanNeverBeInstalled()
		{
			Assert.IsFalse(device.IsAppInstalled(null));
		}

		[Test]
		public void CanOnlyInstallAppIfThereIsOne()
		{
			Assert.Throws<Device.NoAppSpecified>(() => device.Install(null));
			Assert.DoesNotThrow(() => device.Install(new NonExistingApp()));
		}

		[Test]
		public void OnlyValidAppsCanBeUninstalled()
		{
			Assert.Throws<Device.NoAppSpecified>(() => device.Uninstall(null));
			var installableApp = new AppInfoTests.AppInfoWithMockDevice(PlatformName.Web);
			Assert.DoesNotThrow(() => device.Uninstall(installableApp));
		}

		[Test]
		public void LaunchOfAnInvalidAppShouldFail()
		{
			Assert.Throws<Device.StartApplicationFailedOnDevice>(() => device.Launch(null));
			var nonExistingApp = new NonExistingApp();
			Assert.Throws<Device.StartApplicationFailedOnDevice>(() => device.Launch(nonExistingApp));
		}
	}
}
