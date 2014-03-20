using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Data container for SampleItems within the SampleBrowser.
	/// </summary>
	public class Sample
	{
		public Sample(string name, SampleCategory category, string solutionFilePath,
			string projectFilePath, string assemblyFilePath)
		{
			Name = name;
			Category = category;
			if (Category == SampleCategory.Game)
				Description = "Sample Game";
			else if (Category == SampleCategory.Tutorial)
				Description = "Tutorial";
			else
				Description = "Visual Test";
			SetImageUrl(name);
			SolutionFilePath = solutionFilePath;
			ProjectFilePath = projectFilePath;
			AssemblyFilePath = assemblyFilePath;
		}

		public string Name { get; private set; }
		public string Description { get; set; }
		public SampleCategory Category { get; private set; }
		public string ImageUrl { get; private set; }
		public string SolutionFilePath { get; private set; }
		public string ProjectFilePath { get; private set; }
		public string AssemblyFilePath { get; private set; }
		public string EntryClass { get; set; }
		public string EntryMethod { get; set; }
		public bool IsFeatured { get; set; }

		public string Title
		{
			get
			{
				var title = Category == SampleCategory.Tutorial ? Name.SplitWords() : Name;
				return IsFeatured ? title + " " + UnicodeBlackStar : title;
			}
		}

		private const string UnicodeBlackStar = "\x2605";

		private void SetImageUrl(string name)
		{
			if (Category == SampleCategory.Tutorial)
				name = "Tutorial";
			else if (Category == SampleCategory.Test)
				name = name.StartsWith(DeltaEngineNamespace) ? GetTestNamespace(name) : "VisualTest";
			ImageUrl = GetIconWebPath() + name + ".png";
		}

		private const string DeltaEngineNamespace = "DeltaEngine.";

		private static string GetTestNamespace(string name)
		{
			int startIndex = name.IndexOf(DeltaEngineNamespace) + DeltaEngineNamespace.Length;
			int endIndex = name.IndexOf(".Tests");
			name = name.Substring(startIndex, endIndex - startIndex).Split('.')[0];
			return name;
		}

		private static string GetIconWebPath()
		{
			return "http://DeltaEngine.net/Editor/Icons/";
		}

		public bool ContainsFilterText(string filterText)
		{
			return Name.ContainsCaseInsensitive(filterText) ||
				Title.ContainsCaseInsensitive(filterText) ||
				Category.ToString().ContainsCaseInsensitive(filterText) ||
				Description.ContainsCaseInsensitive(filterText) ||
				AssemblyFilePath.ContainsCaseInsensitive(filterText) ||
				EntryMethod.ContainsCaseInsensitive(filterText);
		}

		public override string ToString()
		{
			return "Sample: " + "Title=" + Title + ", Category=" + Category + ", Description=" +
				Description;
		}
	}
}