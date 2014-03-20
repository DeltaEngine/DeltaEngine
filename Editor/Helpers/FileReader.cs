using System.IO;

namespace DeltaEngine.Editor.Helpers
{
	public class FileReader
	{
		//ncrunch: no coverage start
		public virtual byte[] ReadAllBytes(string filePath)
		{
			return File.ReadAllBytes(filePath);
		}
	}
}