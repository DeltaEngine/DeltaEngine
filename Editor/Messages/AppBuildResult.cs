using System;

namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Result of the BuildService after it finished building the request app.
	/// </summary>
	public class AppBuildResult
	{
		/// <summary>
		/// Need empty constructor for serialization and reconstruction.
		/// </summary>
		protected AppBuildResult() {}

		public AppBuildResult(string projectName, PlatformName platform)
		{
			ProjectName = projectName;
			Platform = platform;
		}

		public string ProjectName { get; private set; }
		public PlatformName Platform { get; private set; }
		public string PackageFileName { get; set; }
		public Guid PackageGuid { get; set; }
		public byte[] PackageFileData { get; set; }

		public override string ToString()
		{
			return GetType().Name + "(" + ProjectName + " for " + Platform + ")";
		}
	}
}