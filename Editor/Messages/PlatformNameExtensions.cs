using System;

namespace DeltaEngine.Editor.Messages
{
	public static class PlatformNameExtensions
	{
		public static PlatformName GetPlatformFromFileExtension(string fileExtension)
		{
			switch (fileExtension.ToLower())
			{
			case ".exe":
				return PlatformName.Windows;
			case ".appx":
				return PlatformName.Windows8;
			case ".xap":
				return PlatformName.WindowsPhone7;
			case ".apk":
				return PlatformName.Android;
			case ".ipa":
				return PlatformName.IOS;
			case ".zip":
				return PlatformName.Web;
			}
			throw new UnsupportedPlatformPackageFileExtension(fileExtension);
		}

		public class UnsupportedPlatformPackageFileExtension : Exception
		{
			public UnsupportedPlatformPackageFileExtension(string fileExtension)
				: base(fileExtension) {}
		}

		public static string GetAppExtension(PlatformName platform)
		{
			switch (platform)
			{
			case PlatformName.Windows:
				return ".exe";
			case PlatformName.Windows8:
				return ".appx";
			case PlatformName.WindowsPhone7:
				return ".xap";
			case PlatformName.Android:
				return ".apk";
			case PlatformName.IOS:
				return ".ipa";
			case PlatformName.Web:
				return ".zip";
			default:
				return ".none";
			}
		}
	}
}