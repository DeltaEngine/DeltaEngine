using System;
using System.Linq;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.WindowsPhone7
{
	[Category("Slow"), Timeout(10000)]
	public class WP7DeviceTests
	{
		[TestFixtureSetUp]
		public void GetEmulatorDeviceAndSampleAppInfo()
		{
			sampleApp = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp",
				PlatformName.WindowsPhone7);
			emulatorDevice = sampleApp.AvailableDevices.FirstOrDefault(device => device.IsEmulator);
		}

		private AppInfo sampleApp;
		private Device emulatorDevice;
		
		[Test]
		public void CheckforExistingEmulator()
		{
			Assert.IsTrue(emulatorDevice.IsEmulator);
			Assert.IsTrue(emulatorDevice is WP7Device);
		}

		[Test]
		public void UninstallOfAnInvalidAppShouldFail()
		{
			Assert.Throws<WP7Device.UninstallationFailedOnDevice>(
				() => emulatorDevice.Uninstall(GetInvalidAppInfo()));
		}

		private static WP7AppInfo GetInvalidAppInfo()
		{
			return new WP7AppInfo("Nothing.nan", Guid.NewGuid(), DateTime.Now);
		}

		[Test]
		public void UninstallApp()
		{
			if (!emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Install(sampleApp);
			Assert.IsTrue(emulatorDevice.IsAppInstalled(sampleApp));
			emulatorDevice.Uninstall(sampleApp);
			Assert.IsFalse(emulatorDevice.IsAppInstalled(sampleApp));
		}

		[Test]
		public void InstallationOfAnInvalidAppShouldFail()
		{
			Assert.Throws<WP7Device.InstallationFailedOnDevice>(
				() => emulatorDevice.Install(GetInvalidAppInfo()));
		}

		[Test]
		public void InstallApp()
		{
			if (emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Uninstall(sampleApp);
			Assert.IsFalse(emulatorDevice.IsAppInstalled(sampleApp));
			emulatorDevice.Install(sampleApp);
			Assert.IsTrue(emulatorDevice.IsAppInstalled(sampleApp));
		}

		[Test]
		public void LaunchOfAnInvalidAppShouldFail()
		{
			Assert.Throws<WP7Device.AppNotInstalled>(() => emulatorDevice.Launch(GetInvalidAppInfo()));
		}

		[Test]
		public void LaunchApp()
		{
			if (!emulatorDevice.IsAppInstalled(sampleApp))
				emulatorDevice.Install(sampleApp);
			emulatorDevice.Launch(sampleApp);
		}
	}
}