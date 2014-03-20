using System;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder.Web
{
	public class WebAppInfo : AppInfo
	{
		public WebAppInfo(string appFilePath, Guid appGuid, DateTime buildDate)
			: base(appFilePath, appGuid, PlatformName.Web, buildDate) {}

		protected override Device[] GetAvailableDevices()
		{
			return new Device[] { new WebDevice() };
		}
	}
}