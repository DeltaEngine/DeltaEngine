using System.IO;
using DeltaEngine.Editor.AppBuilder.Android;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Android
{
	public class AdbPathProviderTests
	{
		[Test, Category("Slow"), Timeout(3000)]
		public void FindAdbPath()
		{
			DeleteSupportFolderForTestIfExists();
			var provider = new AdbPathProvider();
			string adbPath = provider.GetAdbPath();
			Assert.IsTrue(File.Exists(adbPath));
		}

		private static void DeleteSupportFolderForTestIfExists()
		{
			if (Directory.Exists(AdbPathProvider.AndroidSupportFilesFolderName))
				Directory.Delete(AdbPathProvider.AndroidSupportFilesFolderName, true);
		}
	}
}