using System.Windows;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.Emulator
{
	public class Emulator
	{
		public Emulator()
		{
			ImageResourceName = "";
			ScreenPoint = new Point();
			ScreenSize = new Size();
			Orientation = Orientation.Landscape;
			Scale = "100%";
			IsDefault = true;
		}

		public string ImageResourceName { get; private set; }
		public Point ScreenPoint { get; private set; }
		public Size ScreenSize { get; private set; }
		public Orientation Orientation { get; private set; }
		public string Scale { get; private set; }
		public bool IsDefault { get; private set; }

		public Emulator(string imageResourceName, Point screenPoint, Size screenSize,
			Orientation orientation, string scale)
		{
			ImageResourceName = imageResourceName;
			ScreenPoint = screenPoint;
			ScreenSize = screenSize;
			Orientation = orientation;
			Scale = scale;
			IsDefault = false;
		}
	}
}