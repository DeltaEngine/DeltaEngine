using System.Collections.Generic;

namespace DeltaEngine.Editor.LevelEditor
{
	public class LevelEditorDesign
	{
		public LevelEditorDesign()
		{
			ContentName = "MyLevel";
			CustomSize = "64, 64";
			WaveList = new List<string> { "Wave 1", "Wave 2", "Wave 3", "..." };
			WaitTime = "5";
			SpawnInterval = "1.5";
			MaxTime = "10";
			SpawnTypeList = "Cloth, Cloth, Cloth";
			WaveNameList = new List<string> { "Wave Cloth" };
			WaveName = "Wave Cloth";
			Is3D = true;
		}

		public string ContentName { get; set; }
		public string CustomSize { get; set; }
		public List<string> WaveList { get; set; }
		public string WaitTime { get; set; }
		public string SpawnInterval { get; set; }
		public string MaxTime { get; set; }
		public string SpawnTypeList { get; set; }
		public List<string> WaveNameList { get; set; }
		public string WaveName { get; set; }
		public bool Is3D { get; set; }
	}
}