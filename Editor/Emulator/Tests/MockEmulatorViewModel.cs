using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class MockEmulatorViewModel : EmulatorViewModel
	{
		protected override Stream GetDevicesXmlStream()
		{
			var devicesXmlData = new XmlData("Devices");
			devicesXmlData.AddChild(EmulatorTestExtensions.CreateDefaultDeviceData());
			devicesXmlData.AddChild(EmulatorTestExtensions.CreateWindows8DeviceData());
			return new XmlFile(devicesXmlData).ToMemoryStream();
		}

		public override void ChangeEmulator()
		{
			SelectedDevice = AvailableDevices[1];
			SelectedOrientation = Orientation.Portrait;
			SelectedScale = AvailableScales[1];
			base.ChangeEmulator();
		}
	}
}