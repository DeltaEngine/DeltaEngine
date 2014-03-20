namespace DeltaEngine.Editor.Core.Tests
{
	public class MockFileWatcher : FileWatcher
	{
		public void UpdateFile()
		{
			if (watcher.EnableRaisingEvents)
				UpdateFile(null, null);
		}

		public string GetCurrentPath()
		{
			return currentlyWatchedAbsoluteFilePath;
		}
	}
}