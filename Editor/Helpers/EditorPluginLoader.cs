using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Helpers
{
	public class EditorPluginLoader
	{
		public EditorPluginLoader()
			: this(Path.Combine("..", "..")) {}

		public EditorPluginLoader(string pluginBaseDirectory)
		{
			this.pluginBaseDirectory = pluginBaseDirectory;
			UserControlsType = new List<Type>();
		}

		public List<Type> UserControlsType { get; private set; }
		private readonly string pluginBaseDirectory;

		public void FindAndLoadAllPlugins()
		{
			CopyAllEditorPlugins(pluginBaseDirectory);
			FindAllEditorPluginViews();
		}

		private static void CopyAllEditorPlugins(string pluginBaseDirectory)
		{
			var pluginDirectories = Directory.GetDirectories(pluginBaseDirectory);
			foreach (var directory in pluginDirectories)
			{
				var pluginOutputDirectory = Path.Combine(directory, "bin",
					ExceptionExtensions.IsDebugMode ? "Debug" : "Release");
				if (Directory.Exists(pluginOutputDirectory) && !directory.EndsWith("Tests"))
					CopyAllDllsAndPdbsToCurrentDirectory(pluginOutputDirectory);
			}
		}

		private static readonly List<string> FreshlyCopiedFiles = new List<string>();

		private static void CopyAllDllsAndPdbsToCurrentDirectory(string directory)
		{
			var files = Directory.GetFiles(directory);
			foreach (var file in files)
				if (Path.GetExtension(file) == ".dll" || Path.GetExtension(file) == ".pdb")
					CopyPluginIfFileIsNewer(file,
						Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(file)));
		}

		private static void CopyPluginIfFileIsNewer(string sourceFile, string targetFile)
		{
			if (sourceFile.Contains("DeltaEngine.Editor.EmptyEditorPlugin."))
				return;
			if (!FreshlyCopiedFiles.Contains(targetFile))
				FreshlyCopiedFiles.Add(targetFile);
			if (!File.Exists(targetFile) ||
				File.GetLastWriteTime(sourceFile) > File.GetLastWriteTime(targetFile))
				CopyFile(sourceFile, targetFile);
		}

		private static void CopyFile(string sourceFile, string targetFile)
		{
			try
			{
				TryCopyFile(sourceFile, targetFile);
			}
			catch (Exception ex)
			{
				Logger.Error(new FailedToCopyFiles(
					"Failed to copy " + sourceFile + " to editor directory", ex));
			}
		}

		private static void TryCopyFile(string sourceFile, string targetFile)
		{
			File.Copy(sourceFile, targetFile, true);
		}

		private class FailedToCopyFiles : Exception
		{
			public FailedToCopyFiles(string message, Exception inner)
				: base(message, inner) {}
		}

		private void FindAllEditorPluginViews()
		{
			var assemblies = new List<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
			var dllFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
			var editorUpdated = File.GetLastWriteTime("DeltaEngine.Editor.exe");
			foreach (var file in dllFiles)
			{
				var projectName = Path.GetFileNameWithoutExtension(file);
				if (projectName.StartsWith("DeltaEngine.") &&
					projectName != "DeltaEngine.Multimedia.VlcToTexture" &&
					(editorUpdated - File.GetLastWriteTime(file)).TotalDays > 7)
				{
					bool removeFile = !FreshlyCopiedFiles.Contains(file);
					Logger.Warning(file + " looks outdated, it was last updated " +
						File.GetLastWriteTime(file) + ". " + (removeFile ? "File will be removed!" : ""));
					if (removeFile)
						File.Delete(file);
				}
				else if (projectName.StartsWith("DeltaEngine.Editor.") &&
					!assemblies.Any(assembly => assembly.FullName.Contains(projectName)))
					assemblies.Add(Assembly.LoadFile(file));
			}
			foreach (var assembly in assemblies)
				if (assembly.IsAllowed())
					AddEditorPlugins(assembly);
		}

		private void AddEditorPlugins(Assembly assembly)
		{
			try
			{
				TryAddEditorPlugins(assembly);
			}
			catch (ReflectionTypeLoadException ex)
			{
				Logger.Warning("Failed to get EditorPluginViews from " + assembly + ": " +
					ex.LoaderExceptions.ToText());
			}
		}

		private void TryAddEditorPlugins(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
				if (typeof(EditorPluginView).IsAssignableFrom(type) &&
					typeof(UserControl).IsAssignableFrom(type))
					UserControlsType.Add(type);
		}
	}
}