using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.WindowsPhone7
{
	public class WP7AppInfo : AppInfo
	{
		public WP7AppInfo(string fullAppDataFilePath, Guid appGuid, DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, PlatformName.WindowsPhone7, buildDate)
		{
			if (deviceFinder == null)
				deviceFinder = new WP7DeviceFinder();
		}

		private static WP7DeviceFinder deviceFinder;

		protected override Device[] GetAvailableDevices()
		{
			return deviceFinder.GetAvailableDevices();
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