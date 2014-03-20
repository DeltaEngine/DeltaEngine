using System.Collections.Generic;

namespace DeltaEngine.Editor.Emulator
{
	public class EmulatorControlDesign
	{
		public EmulatorControlDesign()
		{
			AvailableDevices = new List<string> { "Device .." };
			SelectedDevice = "Device ..";
			AvailableOrientations = new List<string> { "Landscape" };
			SelectedOrientation = "Landscape";
			AvailableScales = new List<string> { "Scaling .." };
			SelectedScale = "Scaling ..";
			AvailableBgImages = new List<string> { "BG Image" };
			SelectedBgImage = "BG Image";
			AvailableBgModels = new List<string> { "BG Model" };
			SelectedBgModel = "BG Model";
		}

		public List<string> AvailableDevices { get; set; }
		public string SelectedDevice { get; set; }
		public List<string> AvailableOrientations { get; set; }
		public string SelectedOrientation { get; set; }
		public List<string> AvailableScales { get; set; }
		public string SelectedScale { get; set; }
		public string SelectedGridSize { get; set; }
		public List<string> AvailableBgImages { get; set; }
		public string SelectedBgImage { get; set; }
		public List<string> AvailableBgModels { get; set; }
		public string SelectedBgModel { get; set; }
	}
}