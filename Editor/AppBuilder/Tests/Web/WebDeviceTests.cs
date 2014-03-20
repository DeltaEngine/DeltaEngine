using DeltaEngine.Editor.AppBuilder.Web;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Web
{
	[Ignore]
	public class WebDeviceTests
	{
		[TestFixtureSetUp]
		public void InitializeDevice()
		{
			device = new WebDevice();
		}

		private Device device;

		[Test]
		public void CheckExistingDevice()
		{
			Assert.IsNotEmpty(device.Name);
			Assert.IsTrue(device.ToString().Contains(device.Name));
		}

		[Test]
		public void UninstallOfAnInvalidAppShouldFail()
		{
			Assert.Throws<WebDevice.UninstallationFailedOnDevice>(() => device.Uninstall(null));
		}

		[Test, Category("Slow"), Timeout(10000)]
		public void UninstallApp()
		{
			Assert.DoesNotThrow(() => device.Uninstall(GetLogoAppForWeb()));
		}

		private static AppInfo GetLogoAppForWeb()
		{
			return AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Web);
		}

		//[Test, Category("Slow")]
		//public void InstallApp()
		//{
		//	if (device.IsAppInstalled(sampleApp))
		//		device.Uninstall(sampleApp);
		//	Assert.IsFalse(firstDevice.IsAppInstalled(sampleApp));
		//	firstDevice.Install(sampleApp);
		//	Assert.IsTrue(firstDevice.IsAppInstalled(sampleApp));
		//}

		//[Test, Category("Slow")]
		//public void LaunchApp()
		//{
		//	if (!device.IsAppInstalled(sampleApp))
		//		device.Install(sampleApp);
		//	device.Launch(sampleApp);
		//}
	}
}
