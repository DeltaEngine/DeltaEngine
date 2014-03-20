using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator
{
	public class EmulatorViewModel
	{
		public EmulatorViewModel()
		{
			EmulatorExtensions.RegisterTypesForConversion();
			LoadDeviceInfoList();
			SetOrientation();
			SetScales();
		}

		public List<Device> AvailableDevices { get; private set; }
		public Device SelectedDevice { get; set; }
		public List<Orientation> AvailableOrientations { get; private set; }
		public Orientation SelectedOrientation { get; set; }
		public List<string> AvailableScales { get; private set; }
		public string SelectedScale { get; set; }

		private void LoadDeviceInfoList()
		{
			AvailableDevices = new List<Device>();
			XmlData devicesXmlData = new XmlFile(GetDevicesXmlStream()).Root;
			foreach (var xmlDevice in devicesXmlData.Children)
				AvailableDevices.Add(new Device(xmlDevice));
			SelectedDevice = AvailableDevices[0];
		}

		protected virtual Stream GetDevicesXmlStream()
		{
			return EmbeddedResourcesLoader.GetEmbeddedResourceStream("Images.Emulators.Devices.xml");
		}

		private void SetOrientation()
		{
			AvailableOrientations = new List<Orientation>();
			AvailableOrientations.Add(Orientation.Landscape);
			AvailableOrientations.Add(Orientation.Portrait);
			SelectedOrientation = AvailableOrientations[0];
		}

		private void SetScales()
		{
			AvailableScales = new List<string>();
			AvailableScales.Add("50%");
			AvailableScales.Add("75%");
			AvailableScales.Add("100%");
			SelectedScale = AvailableScales[2];
		}

		public virtual void ChangeEmulator()
		{
			if (EmulatorChanged != null)
				EmulatorChanged(CreateEmulator());
		}

		private Emulator CreateEmulator()
		{
			if (SelectedDevice.IsDefault())
				return new Emulator();
			return new Emulator(SelectedDevice.ImageFile, SelectedDevice.ScreenPoint,
				SelectedDevice.ScreenSize, SelectedOrientation, SelectedScale);
		}

		public event Action<Emulator> EmulatorChanged;
	}
}