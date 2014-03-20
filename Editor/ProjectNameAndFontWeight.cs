using System;
using System.Windows;

namespace DeltaEngine.Editor
{
	public class ProjectNameAndFontWeight : IEquatable<ProjectNameAndFontWeight>
	{
		public ProjectNameAndFontWeight(string name, FontWeight weight)
		{
			Name = name;
			Weight = weight;
		}

		public string Name { get; private set; }

		public FontWeight Weight { get; private set; }

		public bool Equals(ProjectNameAndFontWeight other)
		{
			return Name == other.Name && Weight == other.Weight;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}