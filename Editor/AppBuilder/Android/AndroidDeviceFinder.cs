namespace DeltaEngine.Editor.AppBuilder.Android
{
	/// <summary>
	/// Finds Android devices which are connected to the current local machine.
	/// </summary>
	/// <see cref="http://www.petefreitag.com/item/763.cfm"/>
	/// <see cref="http://mobile.tutsplus.com/tutorials/connecting-physical-android-devices-to-your-development-machine/"/>
	/// <see cref="http://developer.android.com/tools/devices/emulator.html"/>
	/// <see cref="http://developer.android.com/tools/help/emulator.html"/>
	public class AndroidDeviceFinder
	{
		public Device[] GetAvailableDevices()
		{
			var adbRunner = new AndroidDebugBridgeRunner();
			AndroidDeviceInfo[] deviceInfos = adbRunner.GetInfosOfAvailableDevices();
			var deviceList = new Device[deviceInfos.Length];
			for (int i = 0; i < deviceInfos.Length; i++)
				deviceList[i] = new AndroidDevice(adbRunner, deviceInfos[i]);
			return deviceList;
		}
	}
}