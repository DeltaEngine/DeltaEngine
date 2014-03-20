using System;
using System.IO;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow")]
	public class UnknownDeviceTests
	{
		[SetUp]
		public void CreatePackageFile()
		{
			sampleApp = new UnknownDeviceAppInfo("WhatEverApp.any", Guid.NewGuid(), (PlatformName)1234,
				DateTime.Now);
			File.WriteAllText(sampleApp.FilePath, "Data of UnknownDevice app");
			device = new UnknownDevice();
		}

		private AppInfo sampleApp;
		private UnknownDevice device;

		[TearDown]
		public void DeletePackageFile()
		{
			if (File.Exists(sampleApp.FilePath))
				File.Delete(sampleApp.FilePath);
		}

		[Test]
		public void UninstallWillJustDeleteAppFile()
		{
			Assert.IsTrue(File.Exists(sampleApp.FilePath));
			Assert.IsTrue(IsAppInstalled());
			device.Uninstall(sampleApp);
			Assert.IsFalse(IsAppInstalled());
			Assert.IsFalse(File.Exists(sampleApp.FilePath));
		}

		private bool IsAppInstalled()
		{
			return device.IsAppInstalled(sampleApp);
		}

		[Test]
		public void ExplicitInstallOfAppDoesNotMakeSenseButWillNotCrash()
		{
			Assert.IsTrue(IsAppInstalled());
			device.Install(sampleApp);
			Assert.IsTrue(IsAppInstalled());
		}

		// ncrunch: no coverage start
		[Test, Ignore]
		public void LaunchWillJustOpenDirectory()
		{
			device.Launch(sampleApp);
		}
	}
}