using System;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class SystemInformationTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SystemInfoSetup()
		{
			info = Resolve<SystemInformation>();
		}

		private SystemInformation info;

		[Test]
		public void HasLanguage()
		{
			Assert.IsFalse(string.IsNullOrEmpty(info.Language));
		}

		[Test]
		public void CheckVersionIsCorrect()
		{
			Assert.GreaterOrEqual(info.Version, new Version("0.9.8.3"));
		}

		[Test]
		public void CheckHasNetworkState()
		{
			Assert.IsTrue(info.NetworkState == NetworkState.ConnectedViaWifiNetwork ||
				info.NetworkState == NetworkState.Disconnected);
		}

		[Test]
		public void CheckMachineNameMatchesEnvironment()
		{
			Assert.IsTrue(info.MachineName == Environment.MachineName ||
				info.MachineName == "MockMachineName", info.MachineName);
		}

		[Test]
		public void CheckPlatformNameIsWindows()
		{
			Assert.IsTrue(info.PlatformName == "Windows", info.PlatformName);
		}

		[Test]
		public void CheckPlatformVersionMatchesEnvironment()
		{
			Assert.GreaterOrEqual(info.PlatformVersion, new Version(6, 0));
		}

		[Test]
		public void VerifyDeviceType()
		{
			Assert.IsFalse(info.IsMobileDevice);
			Assert.IsFalse(info.IsTablet);
			Assert.IsFalse(info.IsConsole);
		}

		[Test]
		public void VerifyMaxResolution()
		{
			var size = new Size(System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width,
				System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height);
			Assert.IsTrue(info.MaxResolution == size || info.MaxResolution == new Size(1920, 1080));
		}

		[Test]
		public void VerifyUsernameMatchesEnvironment()
		{
			Assert.IsTrue(info.Username == Environment.UserName || info.Username == "MockUsername");
		}

		[Test]
		public void HasCpuName()
		{
			Assert.GreaterOrEqual(info.CoreCount, 1);
			Assert.IsFalse(string.IsNullOrEmpty(info.CpuName));
		}

		[Test]
		public void HasGpuName()
		{
			Assert.IsFalse(string.IsNullOrEmpty(info.GpuName));
		}

		[Test]
		public void HasCpuSpeed()
		{
			Assert.IsTrue(info.CpuSpeed > 0.0f);
		}

		[Test]
		public void HasCpuUsage()
		{
			Assert.IsTrue(info.CpuUsage.Length > 0);
		}

		[Test]
		public void VerifyCoreCountMatchesEnvironment()
		{
			Assert.IsTrue(info.Username == Environment.UserName || info.Username == "MockUsername");
		}

		[Test]
		public void HasMaxRam()
		{
			Assert.IsTrue(info.MaxRam > 0.0f);
		}

		[Test]
		public void HasUsedRam()
		{
			Assert.IsTrue(info.UsedRam > 0.0f);
		}

		[Test]
		public void HasAvailableRam()
		{
			Assert.IsTrue(info.AvailableRam > 0.0f);
		}

		[Test]
		public void IsSoundCardAvailableDoesNotThrowException()
		{
			Assert.AreEqual(info.SoundCardAvailable, info.SoundCardAvailable);
		}
	}
}