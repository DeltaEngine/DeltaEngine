using System;
using System.IO;

namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// A message sent to the BuildService which tell it what kind of app it has to build.
	/// </summary>
	public class AppBuildRequest : BuildServiceMessage
	{
		/// <summary>
		/// Need empty constructor for serialization and reconstruction.
		/// </summary>
		protected AppBuildRequest() {}

		public AppBuildRequest(string solutionFileName, string projectName, PlatformName platform,
			byte[] serializedCodeData)
		{
			SolutionFileName = solutionFileName;
			ProjectName = projectName;
			ContentProjectName = projectName;
			Platform = platform;
			PackedCodeData = serializedCodeData;
			ValidateData();
		}

		public string SolutionFileName { get; private set; }
		public string ProjectName { get; private set; }
		public string ContentProjectName { get; set; }
		public PlatformName Platform { get; private set; }
		public byte[] PackedCodeData { get; private set; }
		public bool IsRebuildOfCodeForced { get; set; }

		private void ValidateData()
		{
			if (SolutionFileName == null || !Path.HasExtension(SolutionFileName))
				throw new NoSolutionFileNameSpecified();
			if (String.IsNullOrEmpty(ProjectName))
				throw new NoProjectNameSpecified();
			if (PackedCodeData == null || PackedCodeData.Length == 0)
				throw new NoPackedCodeDataSpecified();
		}

		public class NoSolutionFileNameSpecified : Exception { }
		public class NoProjectNameSpecified : Exception { }
		public class NoPackedCodeDataSpecified : Exception { }
	}
}