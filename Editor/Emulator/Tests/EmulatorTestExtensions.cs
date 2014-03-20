using DeltaEngine.Content.Xml;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public static class EmulatorTestExtensions
	{
		public static XmlData CreateDefaultDeviceData()
		{
			var deviceData = new XmlData("Device");
			deviceData.AddChild("Type", "Default");
			deviceData.AddChild("Name", "Default");
			deviceData.AddChild("ImageFile", "");
			deviceData.AddChild("ScreenPoint", "");
			deviceData.AddChild("ScreenSize", "");
			deviceData.AddChild("CanRotate", "False");
			deviceData.AddChild("CanScale", "False");
			deviceData.AddChild("DefaultScaleIndex", "2");
			return deviceData;
		}

		public static XmlData CreateWindows8DeviceData()
		{
			var deviceData = new XmlData("Device");
			deviceData.AddChild("Type", "Windows");
			deviceData.AddChild("Name", "Windows 8 1080p");
			deviceData.AddChild("ImageFile", "W8Emulator1080p");
			deviceData.AddChild("ScreenPoint", "33,33");
			deviceData.AddChild("ScreenSize", "1920,1080");
			deviceData.AddChild("CanRotate", "True");
			deviceData.AddChild("CanScale", "True");
			deviceData.AddChild("DefaultScaleIndex", "0");
			return deviceData;
		}
	}
}