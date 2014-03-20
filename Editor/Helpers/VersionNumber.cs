using System;
using System.Reflection;

namespace DeltaEngine.Editor.Helpers
{
	public class VersionNumber
	{
		public VersionNumber(string version = null)
		{
			Version = string.IsNullOrEmpty(version)
				? Assembly.GetExecutingAssembly().GetName().Version : new Version(version);
		}

		public Version Version { get; private set; }

		public override string ToString()
		{
			if (string.IsNullOrEmpty(versionText))
				SetVersionNumber(Version.Build == 0 ? 2 : 3);
			return versionText;
		}

		private string versionText;

		private void SetVersionNumber(int numberOfVersionComponents)
		{
			versionText = "v" + Version.ToString(numberOfVersionComponents);
		}
	}
}