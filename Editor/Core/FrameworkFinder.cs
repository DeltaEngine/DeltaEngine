using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Core
{
	public class FrameworkFinder
	{
		public DeltaEngineFramework[] All
		{
			get
			{
				if (!haveFrameworksBeenSearchedAlready)
					SearchAllFrameworksAndDefault();
				return allFrameworks;
			}
		}

		private bool haveFrameworksBeenSearchedAlready;

		protected virtual void SearchAllFrameworksAndDefault()
		{
			var installDirectory = GetInstallPathOrWorkingDirectory();
			var installedFrameworks = new List<DeltaEngineFramework>();
			foreach (var directoryPath in GetDirectories(installDirectory))
			{
				var framework = DeltaEngineFrameworkExtensions.FromString(Path.GetFileName(directoryPath));
				if (framework == DeltaEngineFramework.Default || !IsValid(directoryPath))
					continue;
				installedFrameworks.Add(framework);
				if (Path.GetFileName(directoryPath) == "OpenTK")
					defaultFramework = DeltaEngineFramework.OpenTK;
			}
			allFrameworks = installedFrameworks.ToArray();
			haveFrameworksBeenSearchedAlready = true;
		}

		private static string GetInstallPathOrWorkingDirectory()
		{
			var installDirectory = PathExtensions.GetDeltaEngineInstalledDirectory();
			if (installDirectory != null)
				return installDirectory;
			Logger.Warning("Environment Variable '" + PathExtensions.EnginePathEnvironmentVariableName +
				"' not set. Please use the DeltaEngine installer to set it up.");
			string workingDirectory = Directory.GetCurrentDirectory();
			string parentDirectory = Path.GetDirectoryName(workingDirectory);
			return IsFrameworkDirectory(workingDirectory) ? parentDirectory : workingDirectory;
		}

		private static bool IsFrameworkDirectory(string directoryPath)
		{
			string directoryName = Path.GetFileName(directoryPath);
			foreach (DeltaEngineFramework framework in Enum.GetValues(typeof(DeltaEngineFramework)))
				if (framework.ToString() == directoryName)
					return true; //ncrunch: no coverage
			return false;
		}

		//ncrunch: no coverage start
		protected virtual IEnumerable<string> GetDirectories(string parentDirectory)
		{
			return Directory.GetDirectories(parentDirectory);
		} //ncrunch: no coverage end

		private bool IsValid(string directoryPath)
		{
			int numberOfProperFolders = 0;
			foreach (string directory in GetDirectories(directoryPath))
			{
				string directoryName = Path.GetFileName(directory);
				if (directoryName == "Samples" || directoryName == "VisualStudioTemplates")
					numberOfProperFolders++;
			}
			return numberOfProperFolders == 2;
		}

		private DeltaEngineFramework defaultFramework;
		private DeltaEngineFramework[] allFrameworks;

		public DeltaEngineFramework Default
		{
			get
			{
				if (!haveFrameworksBeenSearchedAlready)
					SearchAllFrameworksAndDefault();
				return defaultFramework;
			}
		}
	}
}