using System;
using System.IO;

namespace DeltaEngine.Editor.ProjectCreator
{
	public static class CsProjectExtensions
	{
		public static string GetProjectGuid(string projectFilePath)
		{
			if (String.IsNullOrWhiteSpace(projectFilePath))
				throw new NoProjectFilePathSpecified();
			if (!projectFilePath.EndsWith(".csproj"))
				throw new SpecifiedFileIsNoProjectFile(projectFilePath);
			if (!File.Exists(projectFilePath))
				throw new ProjectFileDoesNotExistsAtSpecifiedPath(projectFilePath);
			return GetExtractedProjectGuid(File.ReadAllText(projectFilePath));
		}

		public class NoProjectFilePathSpecified : Exception { }

		public class SpecifiedFileIsNoProjectFile : Exception
		{
			public SpecifiedFileIsNoProjectFile(string filePath)
				: base(filePath) {}
		}

		public class ProjectFileDoesNotExistsAtSpecifiedPath : Exception
		{
			public ProjectFileDoesNotExistsAtSpecifiedPath(string projectFilePath)
				: base(projectFilePath) {}
		}

		private static string GetExtractedProjectGuid(string csprojFileData)
		{
			const string ProjectGuidOpenNode = "<ProjectGuid>";
			int startIndex = csprojFileData.IndexOf(ProjectGuidOpenNode) + ProjectGuidOpenNode.Length;
			int endIndex = csprojFileData.IndexOf("</ProjectGuid>");
			return csprojFileData.Substring(startIndex, endIndex - startIndex);
		}
	}
}