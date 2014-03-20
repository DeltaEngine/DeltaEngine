using System.Threading;
using DeltaEngine.Editor.AppBuilder.Windows;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Windows
{
	[Category("Slow"), Timeout(15000)]
	public class WindowsDeviceTests
	{
		[TestFixtureSetUp]
		public void CreatePackageFileOfSample()
		{
			sampleApp = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			device = new WindowsDevice();
		}

		private AppInfo sampleApp;
		private WindowsDevice device;

		[Test]
		public void UninstallApp()
		{
			if (!device.IsAppInstalled(sampleApp))
				InstallAppAndWaitHalfASecond(sampleApp);
			Assert.IsTrue(device.IsAppInstalled(sampleApp));
			device.Uninstall(sampleApp);
			Assert.IsFalse(device.IsAppInstalled(sampleApp));
		}

		private void InstallAppAndWaitHalfASecond(AppInfo app)
		{
			device.Install(app);
			Thread.Sleep(500);
		}

		[Test]
		public void InstallApp()
		{
			if (device.IsAppInstalled(sampleApp))
				device.Uninstall(sampleApp);
			InstallAppAndWaitHalfASecond(sampleApp);
			Assert.IsTrue(device.IsAppInstalled(sampleApp));
		}

		[Test]
		public void LaunchApp()
		{
			if (!device.IsAppInstalled(sampleApp))
				InstallAppAndWaitHalfASecond(sampleApp);
			device.Launch(sampleApp);
		}
	}
}