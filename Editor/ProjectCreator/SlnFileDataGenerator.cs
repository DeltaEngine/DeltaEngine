using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.ProjectCreator
{
	public class SlnFileDataGenerator
	{
		public SlnFileDataGenerator(string projectFilePath, string workingDirectory)
		{
			if (Path.IsPathRooted(projectFilePath))
				throw new ProjectFilePathMustBeRelativeToSolutionPath(workingDirectory);
			this.projectFilePath = projectFilePath;
			projectGuid =
				CsProjectExtensions.GetProjectGuid(Path.Combine(workingDirectory, projectFilePath));
		}

		public class ProjectFilePathMustBeRelativeToSolutionPath : Exception
		{
			public ProjectFilePathMustBeRelativeToSolutionPath(string solutionDirectory)
				: base(solutionDirectory) {}
		}

		private readonly string projectFilePath;
		private readonly string projectGuid;

		public string GenerateVisualStudioSlnFileData()
		{
			slnFileTextLines = new List<string>();
			AddVisualStudioFileHeader();
			AddVisualStudioProjectReferenceInfo();
			AddVisualStudioGlobalRootSection();
			return GetLinesAsText();
		}

		private List<string> slnFileTextLines;

		private void AddVisualStudioFileHeader()
		{
			slnFileTextLines.Add("Microsoft Visual Studio Solution File, Format Version 12.00");
			slnFileTextLines.Add("# Visual Studio 2013");
			slnFileTextLines.Add("VisualStudioVersion = 12.0.21005.1");
			slnFileTextLines.Add("MinimumVisualStudioVersion = 10.0.40219.1");
		}

		private void AddVisualStudioProjectReferenceInfo()
		{
			string projectName = Path.GetFileNameWithoutExtension(projectFilePath);
			string projectInfo = "\"" + projectName + "\", \"" + projectFilePath + "\", \"" +
				projectGuid + "\"";
			slnFileTextLines.Add("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}\") = " + projectInfo);
			slnFileTextLines.Add("EndProject");
		}

		private void AddVisualStudioGlobalRootSection()
		{
			slnFileTextLines.Add("Global");
			AddVisualStudioGlobalSolutionConfigurationPlatformsSection();
			AddVisualStudioGlobalSolutionProjectConfigurationPlatformsSection();
			AddVisualStudioGlobalSolutionSolutionPropertiesSection();
			slnFileTextLines.Add("EndGlobal");
		}

		private void AddVisualStudioGlobalSolutionConfigurationPlatformsSection()
		{
			slnFileTextLines.Add("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			slnFileTextLines.Add("\t\tDebug|Any CPU = Debug|Any CPU");
			slnFileTextLines.Add("\t\tRelease|Any CPU = Release|Any CPU");
			slnFileTextLines.Add("\tEndGlobalSection");
		}

		private void AddVisualStudioGlobalSolutionProjectConfigurationPlatformsSection()
		{
			slnFileTextLines.Add("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			slnFileTextLines.Add("\t\t" + projectGuid + ".Debug|Any CPU.ActiveCfg = Debug|Any CPU");
			slnFileTextLines.Add("\t\t" + projectGuid + ".Debug|Any CPU.Build.0 = Debug|Any CPU");
			slnFileTextLines.Add("\t\t" + projectGuid + ".Release|Any CPU.ActiveCfg = Release|Any CPU");
			slnFileTextLines.Add("\t\t" + projectGuid + ".Release|Any CPU.Build.0 = Release|Any CPU");
			slnFileTextLines.Add("\tEndGlobalSection");
		}

		private void AddVisualStudioGlobalSolutionSolutionPropertiesSection()
		{
			slnFileTextLines.Add("\tGlobalSection(SolutionProperties) = preSolution");
			slnFileTextLines.Add("\t\tHideSolutionNode = FALSE");
			slnFileTextLines.Add("\tEndGlobalSection");
		}

		private string GetLinesAsText()
		{
			string finalText = slnFileTextLines[0];
			for (int i = 1; i < slnFileTextLines.Count; i++)
				finalText += Environment.NewLine + slnFileTextLines[i];
			return finalText;
		}
	}
}