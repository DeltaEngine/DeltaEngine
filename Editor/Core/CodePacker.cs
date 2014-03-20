using System;
using System.Collections.Generic;
using System.IO;

namespace DeltaEngine.Editor.Core
{
	public class CodePacker
	{
		public CodePacker(string slnFilePath, string projectNameInSolution)
			: this(GetCodeDirectoryOfProject(slnFilePath, projectNameInSolution)) {}

		private static string GetCodeDirectoryOfProject(string slnFilePath,
			string projectNameInSolution)
		{
			var solutionLoader = new SolutionFileLoader(slnFilePath);
			ProjectEntry searchedProject = solutionLoader.GetCSharpProject(projectNameInSolution);
			string solutionDirectory = Path.GetDirectoryName(slnFilePath);
			string relativeProjectDirectory = Path.GetDirectoryName(searchedProject.FilePath);
			return Path.Combine(solutionDirectory, relativeProjectDirectory);
		}

		public CodePacker(string directoryWithCode)
		{
			this.directoryWithCode = directoryWithCode;
			CollectProjectFilesToPack();
		}

		private readonly string directoryWithCode;

		private void CollectProjectFilesToPack()
		{
			if (!Directory.Exists(directoryWithCode))
				throw new DirectoryDoesNotExist(directoryWithCode);
			CollectedFilesToPack = GetRelevantFiles();
			if (CollectedFilesToPack.Count == 0)
				throw new NoCodeAvailableToPack();
		}

		public class DirectoryDoesNotExist : Exception
		{
			public DirectoryDoesNotExist(string directory)
				: base(directory) {}
		}

		public List<string> CollectedFilesToPack { get; private set; }

		private List<string> GetRelevantFiles()
		{
			var filePathList = new List<string>();
			foreach (string filePath in GetAllFiles(directoryWithCode))
				if (IsNecessaryFile(filePath))
					filePathList.Add(filePath);
			return filePathList;
		}

		private static IEnumerable<string> GetAllFiles(string directory)
		{
			return Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
		}

		private static bool IsNecessaryFile(string filePath)
		{
			return !IsCompilerOutputFile(filePath) && !IsNugetFileOrPackage(filePath) &&
				!IsNoNecessaryCodeFile(filePath);
		}

		private static bool IsCompilerOutputFile(string filePath)
		{
			return filePath.Contains(@"\bin\") || filePath.Contains(@"\obj\");
		}

		private static bool IsNugetFileOrPackage(string filePath)
		{
			return Directory.Exists(".nuget") || filePath.Contains(@"\packages\");
		}

		private static bool IsNoNecessaryCodeFile(string filePath)
		{
			return filePath.EndsWith(".user") || filePath.EndsWith(".suo") ||
				filePath.EndsWith(".cachefile") || filePath.EndsWith(".pch") ||
				filePath.Contains(".ncrunch") || filePath.Contains(@"\Tests\");
		}

		public class NoCodeAvailableToPack : Exception { }

		public byte[] GetPackedData()
		{
			using (var dataStream = new MemoryStream())
			using (var streamWriter = new BinaryWriter(dataStream))
			{
				SaveCollectedFilesToStream(streamWriter);
				return dataStream.ToArray();
			}
		}

		private void SaveCollectedFilesToStream(BinaryWriter streamWriter)
		{
			streamWriter.Write(CollectedFilesToPack.Count);
			foreach (string filePath in CollectedFilesToPack)
				SaveFileToStream(streamWriter, filePath);
		}

		private void SaveFileToStream(BinaryWriter streamWriter, string filePath)
		{
			string relativeFilePath = filePath.Substring(directoryWithCode.Length + 1);
			streamWriter.Write(relativeFilePath);
			byte[] fileBytes = File.ReadAllBytes(filePath);
			streamWriter.Write(fileBytes.Length);
			streamWriter.Write(fileBytes);
		}
	}
}