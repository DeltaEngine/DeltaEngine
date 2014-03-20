using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using NUnit.Framework;
using ProfilingMode = DeltaEngine.Core.ProfilingMode;

namespace DeltaEngine.Tests.Core
{
	public class SettingsTests
	{
		[SetUp]
		public void Init()
		{
			Settings.Current = new MockSettings();
		}

		[Test]
		public void CheckDefaultSettings()
		{
			Settings settings = Settings.Current;
			settings.LoadDefaultSettings();
			Assert.AreEqual(Settings.DefaultResolution, settings.Resolution);
			Assert.AreEqual(false, settings.StartInFullscreen);
			Assert.AreEqual(1.0f, settings.SoundVolume);
			Assert.AreEqual(0.75f, settings.MusicVolume);
			Assert.AreEqual(24, settings.DepthBufferBits);
			Assert.AreEqual(32, settings.ColorBufferBits);
			Assert.AreEqual(0, settings.AntiAliasingSamples);
			Assert.AreEqual(0, settings.LimitFramerate);
			Assert.AreEqual(false, settings.UseVSync);
			Assert.AreEqual(20, settings.UpdatesPerSecond);
			Assert.AreEqual(20, settings.RapidUpdatesPerSecond);
		}

		[Test]
		public void ChangeAndSaveSettings()
		{
			Settings settings = Settings.Current;
			settings.PlayerName = ModifiedPlayerName;
			settings.TwoLetterLanguageName = ModifiedTwoLetterLanguageName;
			settings.StartInFullscreen = false;
			Assert.AreEqual(settings.TwoLetterLanguageName, ModifiedTwoLetterLanguageName);
			Assert.AreEqual(ModifiedPlayerName, settings.PlayerName);
			settings.Save();
		}

		private const string ModifiedPlayerName = "John Doe";

		private const string ModifiedTwoLetterLanguageName = "de";

		[Test]
		public void SetValueTwice()
		{
			Settings settings = Settings.Current;
			settings.PlayerName = "Blub";
			settings.PlayerName = ModifiedPlayerName;
			Assert.AreEqual(ModifiedPlayerName, settings.PlayerName);
		}

		[Test]
		public void EditAndCheckSettings()
		{
			Settings settings = Settings.Current;
			EditSettings(settings);
			Assert.IsTrue((settings as MockSettings).AreChanged);
			CheckSettings(settings);
		}

		[Test]
		public void SettingsChangeBeforeClose()
		{
			MockSettings settings = new MockSettings();
			settings.Change();
			settings.Dispose();
		}

		private static void EditSettings(Settings settings)
		{
			settings.Resolution = new Size(1000, 500);
			settings.SoundVolume = 2.0f;
			settings.MusicVolume = 1.0f;
			settings.DepthBufferBits = 16;
			settings.ColorBufferBits = 16;
			settings.AntiAliasingSamples = 2;
			settings.LimitFramerate = 20;
			settings.UpdatesPerSecond = 30;
			settings.RapidUpdatesPerSecond = 60;
			settings.ProfilingModes = ProfilingMode.Rendering;
			settings.OnlineServiceIp = "content.server.ip";
			settings.OnlineServicePort = 13;
			settings.UseOnlineLogging = false;
		}

		private static void CheckSettings(Settings settings)
		{
			Assert.AreEqual(new Size(1000, 500), settings.Resolution);
			Assert.AreEqual(2.0f, settings.SoundVolume);
			Assert.AreEqual(1.0f, settings.MusicVolume);
			Assert.AreEqual(16, settings.DepthBufferBits);
			Assert.AreEqual(16, settings.ColorBufferBits);
			Assert.AreEqual(2, settings.AntiAliasingSamples);
			Assert.AreEqual(20, settings.LimitFramerate);
			Assert.AreEqual(30, settings.UpdatesPerSecond);
			Assert.AreEqual(60, settings.RapidUpdatesPerSecond);
			Assert.AreEqual(ProfilingMode.Rendering, settings.ProfilingModes);
			Assert.AreEqual("content.server.ip", settings.OnlineServiceIp);
			Assert.AreEqual(13, settings.OnlineServicePort);
			Assert.AreEqual(false, settings.UseOnlineLogging);
		}

		[Test]
		public void CheckMyDocumentsAppFolder()
		{
			string deltaEngineDocumentsFolder = Settings.GetMyDocumentsAppFolder();
			Assert.That(deltaEngineDocumentsFolder,
				Is.StringEnding(@"DeltaEngine\DeltaEngine.Tests"));
		}
	}
}