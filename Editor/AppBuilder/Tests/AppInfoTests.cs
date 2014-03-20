using System;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppInfoTests
	{
		[Test]
		public void LaunchingAnAppWithoutAvailableDeviceShouldThrowAnException()
		{
			var appInfo = new FakeAppInfoWithoutAvailableDevices();
			Assert.Throws<AppInfo.NoDeviceAvailable>(appInfo.LaunchAppOnPrimaryDevice);
		}

		private class FakeAppInfoWithoutAvailableDevices : AppInfo
		{
			public FakeAppInfoWithoutAvailableDevices()
				: base("FakeAppInfoWithoutAvailableDevices", Guid.Empty, (PlatformName)77, DateTime.Now) { }

			protected override Device[] GetAvailableDevices()
			{
				return new Device[0];
			}
		}

		[Test]
		public void CrashOnInternalGetDevicesMethodWillNotLetCrashApp()
		{
			var appInfo = new CrashingAppInfoOnGetAvailableDevices();
			Assert.IsFalse(appInfo.IsDeviceAvailable);
			Assert.IsNull(appInfo.AvailableDevices);
		}

		private class CrashingAppInfoOnGetAvailableDevices : AppInfo
		{
			public CrashingAppInfoOnGetAvailableDevices()
				: base("CrashingAppInfo", Guid.Empty, (PlatformName)77, DateTime.Now) { }

			protected override Device[] GetAvailableDevices()
			{
				throw new Exception("UnknownException");
			}
		}

		[Test]
		public void EachAppInfoProvidesAnIconBasedOnSupportedPlatform()
		{
			var appInfo = new AppInfoWithMockDevice(PlatformName.Windows);
			Assert.IsTrue(appInfo.PlatformIcon.Contains(appInfo.Platform.ToString()));
		}

		internal class AppInfoWithMockDevice : AppInfo
		{
			public AppInfoWithMockDevice(PlatformName platform, string appName = "AppInfoWithMockDevice")
				: base(appName, Guid.Empty, platform, DateTime.Now) { }

			protected override Device[] GetAvailableDevices()
			{
				return new Device[] { new DeviceTests.MockDevice()};
			}
		}

		[Test]
		public void AppCanBeLaunchedOnPrimaryDevice()
		{
			var appInfo = new AppInfoWithMockDevice(PlatformName.Windows);
			Assert.DoesNotThrow(appInfo.LaunchAppOnPrimaryDevice);
			Assert.IsTrue(appInfo.PlatformIcon.Contains(appInfo.Platform.ToString()));
		}

		[Test]
		public void AppInfoProvidesPlatformIcon()
		{
			var appInfo = new AppInfoWithMockDevice(PlatformName.Windows);
			string iconFilePath = appInfo.PlatformIcon;
			Assert.IsTrue(iconFilePath.Contains(appInfo.Platform.ToString()), iconFilePath);
			Assert.IsTrue(iconFilePath.EndsWith(".png"), iconFilePath);
		}
	}
}