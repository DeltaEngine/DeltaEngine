using System;
using System.Collections.Generic;
using Microsoft.SmartDevice.Connectivity;
using MsDevice = Microsoft.SmartDevice.Connectivity.Device;

namespace DeltaEngine.Editor.AppBuilder.WindowsPhone7
{
	internal class WP7DeviceFinder
	{
		public WP7DeviceFinder()
		{
			devicesStorage = new DatastoreManager(1033);
		}

		private readonly DatastoreManager devicesStorage;

		public Device[] GetAvailableDevices()
		{
			var devices = new List<Device>();
			Platform nativePlatform = GetNativePlatform();
			foreach (MsDevice platformDevice in nativePlatform.GetDevices())
				devices.Add(new WP7Device(platformDevice));
			return devices.ToArray();
		}

		private Platform GetNativePlatform()
		{
			foreach (Platform platform in devicesStorage.GetPlatforms())
				if (platform.Name == "Windows Phone 7")
					return platform;
			throw new WindowsPhone7PlatformNotFound(); // ncrunch: no coverage
		}

		public class WindowsPhone7PlatformNotFound : Exception {}
	}
}