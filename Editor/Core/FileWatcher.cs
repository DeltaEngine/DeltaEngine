using System;
using System.IO;
using DeltaEngine.Core;

namespace DeltaEngine.Editor.Core
{
	public class FileWatcher : IDisposable
	{
		public FileWatcher()
		{
			watcher = new FileSystemWatcher();
			SetFileIfSettingsContainWatchedContent();
			watcher.NotifyFilter = NotifyFilters.LastWrite;
			watcher.Changed += UpdateFile;
		}

		protected readonly FileSystemWatcher watcher;

		private void SetFileIfSettingsContainWatchedContent()
		{
			if (Settings.Current == null)
				return;
			var path = Settings.Current.GetValue(SettingsKey, "");
			if (!string.IsNullOrEmpty(path))
				SetFile(path);
		}

		public const string SettingsKey = "WatchedContent";

		public void SetFile(string absoluteFilePath)
		{
			currentlyWatchedAbsoluteFilePath = absoluteFilePath;
			watcher.Path = Path.GetDirectoryName(Path.GetFullPath(absoluteFilePath));
			watcher.Filter = Path.GetFileName(absoluteFilePath);
			watcher.EnableRaisingEvents = true;
		}

		protected string currentlyWatchedAbsoluteFilePath;

		protected void UpdateFile(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (Updated != null)
				Updated();
		}

		public event Action Updated;

		public void Dispose()
		{
			Settings.Current.SetValue(SettingsKey, currentlyWatchedAbsoluteFilePath);
			watcher.Changed -= UpdateFile;
			watcher.Dispose();
		}
	}
}