using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public class UnknownDeviceAppInfo : AppInfo
	{
		public UnknownDeviceAppInfo(string fullAppDataFilePath, Guid appGuid, PlatformName platform,
			DateTime buildDate)
			: base(fullAppDataFilePath, appGuid, platform, buildDate) {}

		protected override Device[] GetAvailableDevices()
		{
			return new Device[] { new UnknownDevice() };
		}
	}
}