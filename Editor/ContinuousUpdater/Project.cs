using System.IO;

namespace DeltaEngine.Editor.ContinuousUpdater
{
	/// <summary>
	/// Stores the assembly file path and displays just the simplified assembly name for the combo box
	/// </summary>
	public class Project
	{
		public Project(string filePath)
		{
			FilePath = filePath;
		}

		public string FilePath { get; private set; }

		public string Name
		{
			get
			{
				return
					Path.GetFileNameWithoutExtension(Path.GetFileName(FilePath).Replace("DeltaEngine.", ""));
			}
			set { FilePath = value; }
		}

		public override string ToString()
		{
			return Name;
		}
	}
}