using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using Ionic.Zip;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Creates a new Delta Engine project on the drive based on a VisualStudioTemplate (.zip).
	/// </summary>
	public class ProjectCreator
	{
		public ProjectCreator(CsProject project, VsTemplate template, Service service)
		{
			Project = project;
			Template = template;
			this.service = service;
		}

		public CsProject Project { get; private set; }
		public VsTemplate Template { get; private set; }
		private readonly Service service;

		public bool AreAllTemplateFilesAvailable()
		{
			return Template.GetAllFilePathsAsList().All(file => File.Exists(file));
		}

		public void CreateProject()
		{
			if (!IsSourceFileAvailable())
				throw new SourceFileNotAvailable(Template.PathToZip);
			CreateTargetDirectoryHierarchy();
			CopyTemplateFilesToLocation();
			ReplacePlaceholdersWithUserInput();
			CreateSolutionFile();
		}

		public class SourceFileNotAvailable : Exception
		{
			public SourceFileNotAvailable(string pathToZip)
				: base(pathToZip) {}
		}

		public bool IsSourceFileAvailable()
		{
			return Template.PathToZip != string.Empty && ZipFile.IsZipFile(Template.PathToZip);
		}

		private void CreateTargetDirectoryHierarchy()
		{
			Directory.CreateDirectory(Project.OutputDirectory);
			Directory.CreateDirectory(GetAssemblyInfoDirectory());
		}

		private string GetAssemblyInfoDirectory()
		{
			return Path.Combine(Project.OutputDirectory, "Properties");
		}

		private void CopyTemplateFilesToLocation()
		{
			var archive = ZipFile.Read(Template.PathToZip);
			foreach (var entry in
				archive.Entries.Where(x => !x.FileName.Contains("vstemplate") && !x.IsDirectory))
				CopyFileInZipToLocation(entry);
		}

		private string GetAssemblyInfoFilePath()
		{
			return Path.Combine(GetAssemblyInfoDirectory(), "AssemblyInfo.cs");
		}

		private string GetRelativeProjectFilePath()
		{
			return Project.Name + ".csproj";
		}

		private string GetProjectFilePath()
		{
			return Path.Combine(Project.OutputDirectory, GetRelativeProjectFilePath());
		}

		private void CopyFileInZipToLocation(ZipEntry entry)
		{
			string targetFilePath = CreateTargetPathForZipEntry(entry);
			if (File.Exists(targetFilePath))
				return;
			using (var stream = entry.OpenReader())
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				File.WriteAllBytes(targetFilePath, memoryStream.ToArray());
			}
		}

		private string CreateTargetPathForZipEntry(ZipEntry entry)
		{
			var target = Path.Combine(Project.OutputDirectory, entry.FileName).Replace('/', '\\');
			if (target.EndsWith(".cs"))
				return target;
			string oldFileName = Path.GetFileName(entry.FileName);
			string newFileName = oldFileName.Replace(Template.Name, Project.Name);
			return target.Replace(oldFileName, newFileName);
		}

		private void ReplacePlaceholdersWithUserInput()
		{
			ReplaceAssemblyInfo();
			ReplaceCsproj();
			foreach (var filename in Template.SourceCodeFiles)
				ReplaceSourceCodeFile(Path.GetFileName(filename));
		}

		private void ReplaceAssemblyInfo()
		{
			var oldFile = File.ReadAllLines(GetAssemblyInfoFilePath());
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$projectname$", Project.Name));
			replacements.Add(new Replacement("$guid1$", Guid.NewGuid().ToString()));
			var newFile = ReplaceFile(oldFile, replacements);
			File.WriteAllText(GetAssemblyInfoFilePath(), newFile);
		}

		private static string ReplaceFile(IEnumerable<string> fileContent,
			List<Replacement> replacements)
		{
			var newFile = new StringBuilder();
			foreach (string line in fileContent)
				newFile.Append(ReplaceLine(line, replacements) + Environment.NewLine);
			return newFile.ToString();
		}

		private static string ReplaceLine(string line, IEnumerable<Replacement> replacements)
		{
			return replacements.Aggregate(line,
				(current, replacement) => current.Replace(replacement.OldValue, replacement.NewValue));
		}

		private void ReplaceCsproj()
		{
			var oldFile = File.ReadAllLines(GetProjectFilePath());
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$guid1$", ""));
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			foreach (var icon in Template.Icons)
			{
				if (icon.EndsWith(".ico"))
				{
					replacements.Add(
						new Replacement("<ApplicationIcon>" + Path.GetFileName(icon) + "</ApplicationIcon>",
							"<ApplicationIcon>" + Project.Name + ".ico</ApplicationIcon>"));
					replacements.Add(new Replacement("\"" + Path.GetFileName(icon) + "\"",
						"\"" + Project.Name + ".ico\""));
				}
				if (icon.EndsWith("Icon72x72.png"))
					replacements.Add(new Replacement("\"" + Path.GetFileName(icon) + "\"",
						"\"" + Project.Name + "Icon72x72.png\""));
			}
			replacements.AddRange(GetReplacementsDependingOnFramework());
			var newFile = ReplaceFile(oldFile, replacements);
			File.WriteAllText(GetProjectFilePath(), newFile);
		}

		private IEnumerable<Replacement> GetReplacementsDependingOnFramework()
		{
			var replacements = new Replacement[2];
			replacements[0] = new Replacement(DeltaEngineFramework.OpenTK.ToInternalName(),
				Project.Framework.ToInternalName());
			replacements[1] = new Replacement(DeltaEngineFramework.OpenTK.ToString(),
				Project.Framework.ToString());
			return replacements;
		}

		private void ReplaceSourceCodeFile(string sourceFileName)
		{
			var oldLines = File.ReadAllLines(Path.Combine(Project.OutputDirectory, sourceFileName));
			var replacements = new List<Replacement>();
			replacements.Add(new Replacement("$safeprojectname$", Project.Name));
			var newLines = ReplaceFile(oldLines, replacements);
			if (newLines != oldLines.ToText(Environment.NewLine))
				File.WriteAllText(Path.Combine(Project.OutputDirectory, sourceFileName), newLines);
		}

		private void CreateSolutionFile()
		{
			var fileDataGenerator = new SlnFileDataGenerator(GetRelativeProjectFilePath(),
				Project.OutputDirectory);
			string slnFilePath = Path.Combine(Project.OutputDirectory, Project.Name + ".sln");
			string slnFileData = fileDataGenerator.GenerateVisualStudioSlnFileData();
			File.WriteAllText(slnFilePath, slnFileData);
			service.SetContentProjectSolutionFilePath(Project.Name, slnFilePath);
		}

		public bool HasDirectoryHierarchyBeenCreated()
		{
			return Directory.Exists(Project.OutputDirectory) &&
				Directory.Exists(Path.Combine(Project.OutputDirectory, "Properties"));
		}

		public void CheckIfTemplateFilesHaveBeenCopiedToLocation()
		{
			foreach (var file in Template.SourceCodeFiles)
				if (!File.Exists(Path.Combine(Project.OutputDirectory, Path.GetFileName(file))))
					throw new FileNotFoundException(file);
			if (!File.Exists(GetAssemblyInfoFilePath()))
				throw new FileNotFoundException(GetAssemblyInfoFilePath());
			if (!File.Exists(GetProjectFilePath()))
				throw new FileNotFoundException(GetProjectFilePath());
		}
	}
}