using System.IO;
using DeltaEngine.Editor.AppBuilder.Android;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Android
{
	[Category("Slow"), Timeout(15000)]
	public class AndroidDriverDownloaderTests
	{
		[SetUp]
		public void CreateDownloader()
		{
			driverDownloader = new AndroidDriverDownloader();
		}

		private AndroidDriverDownloader driverDownloader;

		[Test]
		public void DownloadDirectoryMayNotBeEmptyAfterDownload()
		{
			if (Directory.Exists(driverDownloader.DownloadDirectory))
				Directory.Delete(driverDownloader.DownloadDirectory, true);
			driverDownloader.DownloadAndroidDriver();
			Assert.IsNotEmpty(Directory.GetFiles(driverDownloader.DownloadDirectory));
		}

		[Test]
		public void AfterDriverDownloadThereShouldBeAlsoInstructions()
		{
			driverDownloader.DownloadAndroidDriver();
			Assert.DoesNotThrow(driverDownloader.ShowInstructions);
		}
	}
}
