using System.Windows;
using DeltaEngine.Content.Xml;

namespace DeltaEngine.Editor.Emulator
{
	public class Device
	{
		public Device(XmlData xmlDeviceData)
		{
			Type = xmlDeviceData.GetChildValue("Type", "Default");
			Name = xmlDeviceData.GetChildValue("Name", "Default");
			ImageFile = xmlDeviceData.GetChildValue("ImageFile", "");
			ScreenPoint = xmlDeviceData.GetChildValue("ScreenPoint", new Point(0, 0));
			ScreenSize = xmlDeviceData.GetChildValue("ScreenSize", new Size(0, 0));
			CanRotate = xmlDeviceData.GetChildValue("CanRotate", false);
			CanScale = xmlDeviceData.GetChildValue("CanScale", false);
			DefaultScaleIndex = xmlDeviceData.GetChildValue("DefaultScaleIndex", 2);
		}

		public string Type { get; private set; }
		public string Name { get; private set; }
		public string ImageFile { get; private set; }
		public Point ScreenPoint { get; private set; }
		public Size ScreenSize { get; private set; }
		public bool CanRotate { get; private set; }
		public bool CanScale { get; private set; }
		public int DefaultScaleIndex { get; private set; }

		public override string ToString()
		{
			return Type + (IsDefault() ? "" : " - " + Name);
		}

		public bool IsDefault()
		{
			return Type == "Default" || Name == "Default";
		}
	}
}