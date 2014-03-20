using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeltaEngine.Editor.Core
{
	[DebuggerDisplay("ProjectEntry={Name}")]
	public class ProjectEntry : IEquatable<ProjectEntry>
	{
		public ProjectEntry(string projectEntryString)
		{
			string[] dataParts = GetProjectDataElements(projectEntryString);
			TypeGuid = dataParts[0];
			Name = dataParts[1];
			FilePath = dataParts[2];
			Guid = dataParts[3];
		}

		private static string[] GetProjectDataElements(string projectString)
		{
			// Do our own SplitAndTrim here as long as we are not allowed to have any reference to the
			// DeltaEngine assemblies
			string[] elements = projectString.Split(new [] { "Project(", "\"", ") =", "," },
				StringSplitOptions.None);
			var nonEmptyElements = new List<string>();
			for (int i = 0; i < elements.Length; i++)
			{
				string trimmedElement = elements[i].Trim();
				if (trimmedElement.Length > 0)
					nonEmptyElements.Add(trimmedElement);
			}
			return nonEmptyElements.ToArray();
		}

		public string TypeGuid { get; private set; }
		public string Name { get; private set; }
		public string FilePath { get; private set; }
		public string Guid { get; private set; }

		public bool IsCSharpProject
		{
			get { return TypeGuid == CSharpProjectTypeGuid; }
		}

		public const string CSharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";

		public bool IsSolutionFolder
		{
			get { return TypeGuid == ProjectFolderGuid; }
		}

		public const string ProjectFolderGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

		public bool Equals(ProjectEntry other)
		{
			return other != null && other.Name == Name && other.FilePath == FilePath;
		}

		public override bool Equals(object other)
		{
			return other is ProjectEntry && Equals((ProjectEntry)other);
		}

		public override int GetHashCode()
		{
			return Guid.GetHashCode();
		}
	}
}