using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.ContinuousUpdater.Tests
{
	public class MockProjectParser : ProjectParser
	{
		protected override IEnumerable<string> GetFiles(string directory, string filter)
		{
			var files = new List<string>();
			if (!directory.Contains("Tests") && !directory.Contains("LogoApp"))
				return files;
			if (filter == "*.dll")
			{
				files.Add(Path.Combine(directory, "Autofac.dll"));
				files.Add(Path.Combine(directory, "DeltaEngine.Graphics.dll"));
				files.Add(Path.Combine(directory, "DeltaEngine.Input.dll"));
				files.Add(Path.Combine(directory, "DeltaEngine.Platforms.dll"));
				files.Add(Path.Combine(directory, "Tests.dll"));
			}
			else if (directory.Contains("LogoApp"))
				files.Add(Path.Combine(directory, "LogoApp.exe"));
			return files;
		}
	}
}