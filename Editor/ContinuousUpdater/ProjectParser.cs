using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.ContinuousUpdater
{
	public class ProjectParser
	{
		public List<Project> Parse(string directory)
		{
			projectName = GetDirectoryNameTwoPathsUp(directory);
			CheckAssemblyFiles(directory, "*.dll");
			CheckAssemblyFiles(directory, "*.exe");
			return new List<Project>();
		}

		private string projectName;

		private static string GetDirectoryNameTwoPathsUp(string directory)
		{
			return Path.GetFileName(Path.GetFullPath(Path.Combine(directory, "..", "..")));
		}

		private void CheckAssemblyFiles(string directory, string filter)
		{
			foreach (var dllFile in GetFiles(directory, filter))
				CheckAssembly(dllFile);
		}

		protected virtual IEnumerable<string> GetFiles(string directory, string filter)
		{
			return Directory.GetFiles(directory, filter);
		}

		protected virtual bool CheckAssembly(string assemblyPath)
		{
			AssembliesChecked++;
			bool result = assemblyPath.EndsWith(projectName + Path.GetExtension(assemblyPath));
			if (result)
				AssembliesMatching++;
			return result;
		}

		public int AssembliesChecked { get; private set; }
		public int AssembliesMatching { get; private set; }
	}
}