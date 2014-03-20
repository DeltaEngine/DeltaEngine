using System.Collections.Generic;

namespace DeltaEngine.Editor.SampleBrowser
{
	public class SampleBrowserDesign
	{
		public SampleBrowserDesign()
		{
			AssembliesAvailable = new List<string>(new[] { "SampleGames" });
			SelectedAssembly = "SampleGames";
			FrameworksAvailable = new List<string>(new[] { "OpenTK" });
			SelectedFramework = "OpenTK";
			Samples = new List<Sample>
			{
				new Sample("Asteroids", SampleCategory.Game, "", "", ""),
				new Sample("Visual Test", SampleCategory.Test, "", "", ""),
				new Sample("Tutorial", SampleCategory.Tutorial, "", "", "")
			};
		}

		public List<string> AssembliesAvailable { get; set; }
		public string SelectedAssembly { get; set; }
		public List<string> FrameworksAvailable { get; set; }
		public string SelectedFramework { get; set; }
		public List<Sample> Samples { get; set; }
	}
}