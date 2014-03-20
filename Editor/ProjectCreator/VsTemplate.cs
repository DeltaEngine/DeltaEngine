using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using Ionic.Zip;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Template ZIP-File from which a new Delta Engine C# project will be created.
	/// </summary>
	public class VsTemplate
	{
		public VsTemplate(string templateName)
		{
			Name = templateName;
			templatePath = Path.Combine("VisualStudioTemplates", "Delta Engine", templateName + ".zip");
			PathToZip = GetPathToVisualStudioTemplateZip(templateName);
			var basePath = GetBasePath(PathToZip);
			AssemblyInfo = Path.Combine(basePath, "Properties", "AssemblyInfo.cs");
			Csproj = Path.Combine(basePath, templateName + ".csproj");
			foreach (var entry in ZipFile.Read(PathToZip).Entries)
			{
				if (entry.FileName.EndsWith(".ico") || entry.FileName.EndsWith(".png"))
					Icons.Add(Path.Combine(basePath, entry.FileName));
				if (!entry.FileName.Contains("AssemblyInfo.cs") && entry.FileName.EndsWith(".cs"))
					SourceCodeFiles.Add(Path.Combine(basePath, entry.FileName));
			}
		}

		public string Name { get; private set; }
		private readonly string templatePath;

		public string PathToZip { get; private set; }

		private string GetPathToVisualStudioTemplateZip(string templateName)
		{
			var currentPath = GetVstFromCurrentWorkingDirectory();
			if (File.Exists(currentPath))
				return currentPath; //ncrunch: no coverage
			var solutionPath = GetVstFromSolution();
			if (File.Exists(solutionPath))
				return solutionPath; //ncrunch: no coverage
			var environmentPath = GetVstFromEnvironmentVariable();
			return File.Exists(environmentPath)
				? environmentPath
				: MyDocumentsExtensions.GetVisualStudioDeltaEngineTemplateZip(templateName);
		}

		private string GetVstFromCurrentWorkingDirectory()
		{
			return
				Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), templatePath));
		}

		private string GetVstFromSolution()
		{
			return
				Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..",
					templatePath));
		}

		private string GetVstFromEnvironmentVariable()
		{
			return Path.Combine(Environment.ExpandEnvironmentVariables("%DeltaEnginePath%"),
				templatePath);
		}

		private static string GetBasePath(string fileName)
		{
			return fileName == string.Empty ? "" : Path.GetDirectoryName(fileName);
		}

		public string AssemblyInfo { get; private set; }
		public string Csproj { get; private set; }
		public readonly List<string> Icons = new List<string>();
		public readonly List<string> SourceCodeFiles = new List<string>();

		public List<string> GetAllFilePathsAsList()
		{
			var list = new List<string> { Csproj, AssemblyInfo };
			list.AddRange(Icons);
			list.AddRange(SourceCodeFiles);
			return list;
		}

		public static string[] GetAllTemplateNames(DeltaEngineFramework framework)
		{
			if (!PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
			{ //ncrunch: no coverage start
				Logger.Warning("No Visual Studio Templates found, please use the Installer to set them up");
				return new string[0];
			} //ncrunch: no coverage end
			var templatePath = Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(),
				framework.ToString(), "VisualStudioTemplates", "Delta Engine");
			var templateNames = new List<string>();
			foreach (var file in Directory.GetFiles(templatePath))
				templateNames.Add(Path.GetFileNameWithoutExtension(file));
			templateNames.Remove("EmptyLibrary");
			return templateNames.ToArray();
		}
	}
}