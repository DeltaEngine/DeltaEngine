using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Mocks
{
	public class FrameworkFinderSpy : FrameworkFinder
	{
		public FrameworkFinderSpy()
		{
			NumberOfSearches = 0;
		}

		public int NumberOfSearches { get; private set; }

		protected override void SearchAllFrameworksAndDefault()
		{
			base.SearchAllFrameworksAndDefault();
			NumberOfSearches++;
		}

		protected override IEnumerable<string> GetDirectories(string parentDirectory)
		{
			var frameworks = new List<string>();
			frameworks.AddRange(GetFrameworkDirectories());
			frameworks.AddRange(GetRequiredFrameworkSubDirectories());
			return PotentialFrameworkDirectories = frameworks.ToArray();
		}

		private static IEnumerable<string> GetFrameworkDirectories()
		{
			var frameworkDirectories = new List<string>();
			foreach (DeltaEngineFramework framework in Enum.GetValues(typeof(DeltaEngineFramework)))
				if (framework != DeltaEngineFramework.Default)
					frameworkDirectories.Add(Path.Combine("C:\\", "DeltaEngine", framework.ToString()));
			return frameworkDirectories;
		}

		private static IEnumerable<string> GetRequiredFrameworkSubDirectories()
		{
			var frameworkSubDirectories = new List<string>();
			frameworkSubDirectories.Add("Samples");
			frameworkSubDirectories.Add("VisualStudioTemplates");
			return frameworkSubDirectories;
		}

		public string[] PotentialFrameworkDirectories { get; private set; }
	}
}