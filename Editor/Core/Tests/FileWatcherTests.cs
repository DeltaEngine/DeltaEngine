using System.IO;
using System.Threading;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class FileWatcherTests
	{
		[Test]
		public void GetNotifiedIfFileChanges()
		{
			var fileWatcher = new MockFileWatcher();
			bool isUpdated = false;
			fileWatcher.Updated += () => isUpdated = true;
			fileWatcher.SetFile(FilePath);
			fileWatcher.UpdateFile();
			Assert.IsTrue(isUpdated);
		}

		private const string FilePath = "TestFile.txt";

		[Test]
		public void SaveWatchedFileToSettingsOnDispose()
		{
			var settings = new MockSettings();
			var fileWatcher = new MockFileWatcher();
			fileWatcher.SetFile(FilePath);
			fileWatcher.Dispose();
			Assert.AreEqual(FilePath, settings.GetValue(FileWatcher.SettingsKey, ""));
		}

		[Test]
		public void LoadWatchedFileFromSettingsOnInitialization()
		{
			var settings = new MockSettings();
			string fullFilePath = Path.GetFullPath(FilePath);
			settings.SetValue(FileWatcher.SettingsKey, fullFilePath);
			var fileWatcher = new MockFileWatcher();
			Assert.AreEqual(fullFilePath, fileWatcher.GetCurrentPath());
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void GetNotifiedOnModificationOfWatchedFile()
		{
			MakeSureTestFileIsDeleted();
			CreateTestFile();
			var fileWatcher = new FileWatcher();
			bool isUpdated = false;
			fileWatcher.Updated += () => isUpdated = true;
			fileWatcher.SetFile(FilePath);
			ModifyTestFileAndWait();
			Assert.IsTrue(isUpdated);
		}

		private static void MakeSureTestFileIsDeleted()
		{
			if (File.Exists(FilePath))
				File.Delete(FilePath);
		}

		private static void CreateTestFile()
		{
			using (FileStream fileStream = File.Create(FilePath))
				fileStream.WriteByte(0);
		}

		private static void ModifyTestFileAndWait()
		{
			File.WriteAllText(FilePath, "Hello world!");
			Thread.Sleep(5);
		}
	}
}