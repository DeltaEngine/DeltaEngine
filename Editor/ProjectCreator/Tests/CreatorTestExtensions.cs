using System.IO;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	public static class CreatorTestExtensions
	{
		public static string GetEngineTemplatesDirectory(string framework = "GLFW")
		{
			return Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(), framework,
				"VisualStudioTemplates", "Delta Engine");
		}

		public static string GetDeltaEngineProjectFilePath()
		{
			return Path.Combine(PathExtensions.GetFallbackEngineSourceCodeDirectory(),
				"DeltaEngine.csproj");
		}
	}
}