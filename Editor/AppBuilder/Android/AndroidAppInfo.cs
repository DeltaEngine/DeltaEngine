using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.Android
{
	public class AndroidAppInfo : AppInfo
	{
		public AndroidAppInfo(string fullAppDataFilePath, Guid appGuid, DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, PlatformName.Android, buildDate) { }

		protected override Device[] GetAvailableDevices()
		{
			return new AndroidDeviceFinder().GetAvailableDevices();
		}

		public override bool IsDeviceAvailable
		{
			get
			{
				UpdateAvailableDevices();
				return base.IsDeviceAvailable;
			}
		}
	}
}