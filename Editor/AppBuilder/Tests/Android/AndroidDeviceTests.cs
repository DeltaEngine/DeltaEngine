using System;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Android
{
	[Category("Slow"), Timeout(10000)]
	public class AndroidDeviceTests
	{
		[TestFixtureSetUp]
		public void GetEmulatorDeviceAndSampleAppInfo()
		{
			sampleApp = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Android);
			firstDevice = sampleApp.AvailableDevices[0];
		}

		private AppInfo sampleApp;
		private Device firstDevice;

		[Test]
		public void CheckExistingDevice()
		{
			Console.WriteLine("Device is emulator: " + firstDevice.IsEmulator);
			var availableDevice = firstDevice as AndroidDevice;
			Assert.IsNotNull(availableDevice);
			Assert.IsTrue(availableDevice.IsConnected);
		}

		[Test, Category("Slow")]
		public void UninstallApp()
		{
			if (!firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Install(sampleApp);
			Assert.IsTrue(firstDevice.IsAppInstalled(sampleApp));
			firstDevice.Uninstall(sampleApp);
			Assert.IsFalse(firstDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void InstallApp()
		{
			if (firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Uninstall(sampleApp);
			Assert.IsFalse(firstDevice.IsAppInstalled(sampleApp));
			firstDevice.Install(sampleApp);
			Assert.IsTrue(firstDevice.IsAppInstalled(sampleApp));
		}

		[Test, Category("Slow")]
		public void LaunchApp()
		{
			if (!firstDevice.IsAppInstalled(sampleApp))
				firstDevice.Install(sampleApp);
			firstDevice.Launch(sampleApp);
		}
	}
}