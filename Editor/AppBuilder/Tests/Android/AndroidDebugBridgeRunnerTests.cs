using System;
using System.Linq;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Android
{
	[Category("Slow"), Timeout(16000)]
	public class AndroidDebugBridgeRunnerTests
	{
		[TestFixtureSetUp]
		public void Start()
		{
			adbRunner = new AndroidDebugBridgeRunner();
			firstDevice = GetFirstAvailableDevice();
		}

		private AndroidDebugBridgeRunner adbRunner;
		private AndroidDevice firstDevice;

		private static AndroidDevice GetFirstAvailableDevice()
		{
			return new AndroidDeviceFinder().GetAvailableDevices().Cast<AndroidDevice>().FirstOrDefault();
		}

		[Test]
		public void CheckConnectedDevices()
		{
			AndroidDeviceInfo[] deviceInfos = adbRunner.GetInfosOfAvailableDevices();
			foreach (AndroidDeviceInfo info in deviceInfos)
				AssertAndroidDeviceInfo(info);
		}

		private static void AssertAndroidDeviceInfo(AndroidDeviceInfo deviceInfo)
		{
			Assert.IsNotEmpty(deviceInfo.AdbDeviceId);
			Assert.IsNotEmpty(deviceInfo.DeviceState);
		}

		[Test]
		public void ExpectExceptionForWrongDeviceName()
		{
			Assert.Throws<AndroidDebugBridgeRunner.DeterminationDeviceNameFailed>(
				() => adbRunner.GetDeviceName("MyFakeDevice"));
		}

		[Test]
		public void GetDeviceName()
		{
			AndroidDeviceInfo[] deviceInfos = adbRunner.GetInfosOfAvailableDevices();
			foreach (AndroidDeviceInfo info in deviceInfos)
				AssertAndroidDeviceName(info.AdbDeviceId);
		}

		private void AssertAndroidDeviceName(string adbDeviceId)
		{
			string deviceName = adbRunner.GetDeviceName(adbDeviceId);
			Console.WriteLine("Android device name: '" + deviceName + "' (AdbId=" + adbDeviceId + ")");
			Assert.IsNotEmpty(deviceName);
			Assert.AreNotEqual(deviceName, adbDeviceId);
		}

		[Test]
		public void IsAppInstalled()
		{
			if (firstDevice == null)
				return;
			Assert.IsTrue(IsAppInstalled("com.android.settings"));
			Console.WriteLine("Is LogoApp installed: " + IsAppInstalled("net.DeltaEngine.LogoApp"));
			Console.WriteLine("Is GhostWars installed: " + IsAppInstalled("net.DeltaEngine.GhostWars"));
		}

		private bool IsAppInstalled(string fullAppName)
		{
			return adbRunner.IsAppInstalled(firstDevice, fullAppName);
		}

		[Test]
		public void CheckIfAppIsNotInstalled()
		{
			if (firstDevice != null)
				Assert.IsFalse(IsAppInstalled("non.existing.package"));
		}

		[Test]
		public void InstallationOfAnInvalidAppShouldFail()
		{
			Assert.Throws<AndroidDebugBridgeRunner.InstallationFailedOnDevice>(
				() => adbRunner.InstallPackage(firstDevice, "Nothing"));
		}

		[Test]
		public void InstallSamplePackage()
		{
			if (firstDevice == null)
				return;
			AppInfo app = GetAppInfoForAndroid("LogoApp");
			if (IsAppInstalled(app))
				UninstallPackage(app);
			InstallPackage(app);
			Assert.IsTrue(IsAppInstalled(app));
		}

		private static AppInfo GetAppInfoForAndroid(string appName)
		{
			return AppBuilderTestExtensions.TryGetAlreadyBuiltApp(appName, PlatformName.Android);
		}

		private bool IsAppInstalled(AppInfo app)
		{
			return IsAppInstalled(app.GetFullAppNameForEngineApp());
		}

		private void InstallPackage(AppInfo app)
		{
			adbRunner.InstallPackage(firstDevice, app.FilePath);
		}

		private void UninstallPackage(AppInfo app)
		{
			adbRunner.UninstallPackage(firstDevice, app.GetFullAppNameForEngineApp());
		}

		[Test]
		public void UninstallSamplePackage()
		{
			if (firstDevice == null)
				return;
			AppInfo app = GetAppInfoForAndroid("LogoApp");
			if (!IsAppInstalled(app))
				InstallPackage(app);
			Assert.IsTrue(IsAppInstalled(app));
			UninstallPackage(app);
			Assert.IsFalse(IsAppInstalled(app));
		}

		[Test]
		public void LaunchSampleApplication()
		{
			if (firstDevice == null)
				return;
			AppInfo app = GetAppInfoForAndroid("LogoApp");
			if (!IsAppInstalled(app))
				InstallPackage(app);
			adbRunner.StartEngineBuiltApplication(firstDevice, app.GetFullAppNameForEngineApp());
		}
	}
}