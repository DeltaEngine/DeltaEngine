using System.Collections.Generic;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AppBuilderDesign
	{
		/// <summary>
		/// Helper class for an easier view modeling at design time.
		/// </summary>
		public AppBuilderDesign()
		{
			UserSolutionPath = @"C:\DeltaEngine\OpenTK\DeltaEngine.Samples.sln";
			AvailableProjectsInSelectedSolution = new List<ProjectEntry>();
			AvailableProjectsInSelectedSolution.Add(new ProjectEntry(GetProjectEntryString()));
			SelectedSolutionProject = AvailableProjectsInSelectedSolution[0];
			SupportedPlatforms = new List<string>(new[] { "Windows" });
			SelectedPlatform = SupportedPlatforms[0];
		}

		public string UserSolutionPath { get; set; }

		private static string GetProjectEntryString()
		{
			const string ProjectName = "MyCSharpProject";
			const string ProjectFilePath = @"Basics\Coding.MyCSharpProject.csproj";
			const string ProjectGuid = "{AAE7730E-5F62-48D6-B772-C4B1E8665FE1}";
			return "Project(" + ProjectEntry.CSharpProjectTypeGuid + ") = \"" + ProjectName + "\", \"" +
				ProjectFilePath + "\", " + ProjectGuid;
		}

		public List<ProjectEntry> AvailableProjectsInSelectedSolution { get; set; }
		public ProjectEntry SelectedSolutionProject { get; set; }
		public List<string> SupportedPlatforms { get; set; }
		public string SelectedPlatform { get; set; }
	}
}