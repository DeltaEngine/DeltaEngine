using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	[Category("Slow")]
	public class FileSettingsTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Init()
		{
			StackTraceExtensions.SetUnitTestName(TestContext.CurrentContext.Test.FullName);
			fileSettings = new FileSettings();
		}

		private FileSettings fileSettings;

		[TearDown]
		public void Cleanup()
		{
			if (File.Exists(SettingsFilePath))
				File.Delete(SettingsFilePath);
		}

		private static string SettingsFilePath
		{
			get { return Path.Combine(Settings.GetMyDocumentsAppFolder(), "Settings.xml"); }
		}

		[Test, CloseAfterFirstFrame]
		public void CheckDefaultSettings()
		{
			Assert.AreEqual(false, fileSettings.StartInFullscreen);
			Assert.AreEqual(1.0f, fileSettings.SoundVolume);
			Assert.AreEqual(0.75f, fileSettings.MusicVolume);
			Assert.AreEqual(24, fileSettings.DepthBufferBits);
			Assert.AreEqual(32, fileSettings.ColorBufferBits);
			Assert.AreEqual(0, fileSettings.AntiAliasingSamples);
			Assert.AreEqual(0, fileSettings.LimitFramerate);
			Assert.AreEqual(20, fileSettings.UpdatesPerSecond);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAndSaveSettings()
		{
			fileSettings.PlayerName = ModifiedPlayerName;
			fileSettings.TwoLetterLanguageName = ModifiedTwoLetterLanguageName;
			fileSettings.Save();
			Assert.IsTrue(File.Exists(SettingsFilePath));
		}

		private const string ModifiedPlayerName = "John Doe";
		private const string ModifiedTwoLetterLanguageName = "de";

		[Test]
		public void ChangeFileSettings()
		{
			fileSettings.StartInFullscreen = true;
			Assert.AreEqual(true, fileSettings.StartInFullscreen);
			fileSettings.StartInFullscreen = false;
		}
	}
}