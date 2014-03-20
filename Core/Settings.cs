using System;
using System.IO;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Keeps a bunch of settings like the resolution, which are used when the application starts up.
	/// </summary>
	public abstract class Settings : IDisposable
	{
		public static Settings Current { get; internal set; }
		public abstract void Save();
		protected const string SettingsFilename = "Settings.xml";

		public void Dispose()
		{
			if (wasChanged)
				Save();
		}

		protected bool wasChanged;
		public bool CustomSettingsExists { get; protected set; }
		public virtual void LoadDefaultSettings() {}

		public static string GetMyDocumentsAppFolder()
		{
			var appPath = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DeltaEngine",
				AssemblyExtensions.GetEntryAssemblyForProjectName());
			if (!Directory.Exists(appPath))
				Directory.CreateDirectory(appPath); //ncrunch: no coverage
			return appPath;
		}

		public Size Resolution
		{
			get { return GetValue("Resolution", DefaultResolution); }
			set { SetValue("Resolution", value); }
		}

		public abstract T GetValue<T>(string name, T defaultValue);

		public static Size DefaultResolution
		{
			get
			{
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					return new Size(320, 180);
				//ncrunch: no coverage start
				return ExceptionExtensions.IsDebugMode ? new Size(640, 360) : new Size(1280, 720);
				//ncrunch: no coverage end
			}
		}

		public abstract void SetValue(string name, object value);

		public bool StartInFullscreen
		{
			get { return GetValue("StartInFullscreen", false); }
			set { SetValue("StartInFullscreen", value); }
		}

		public string PlayerName
		{
			get { return GetValue("PlayerName", "Player"); }
			set { SetValue("PlayerName", value); }
		}

		public string TwoLetterLanguageName
		{
			get { return GetValue("Language", "en"); }
			set { SetValue("Language", value); }
		}

		public float SoundVolume
		{
			get { return GetValue("SoundVolume", 1.0f); }
			set { SetValue("SoundVolume", value); }
		}

		public float MusicVolume
		{
			get { return GetValue("MusicVolume", 0.75f); }
			set { SetValue("MusicVolume", value); }
		}

		public int DepthBufferBits
		{
			get { return GetValue("DepthBufferBits", 24); }
			set { SetValue("DepthBufferBits", value); }
		}

		public int ColorBufferBits
		{
			get { return GetValue("ColorBufferBits", 32); }
			set { SetValue("ColorBufferBits", value); }
		}

		/// <summary>
		/// 4 works on most frameworks, but some have AA disabled, use 0 to keep all frameworks in sync
		/// </summary>
		public int AntiAliasingSamples
		{
			get { return GetValue("AntiAliasingSamples", 0); }
			set { SetValue("AntiAliasingSamples", value); }
		}

		public int LimitFramerate
		{
			get { return GetValue("LimitFramerate", 0); }
			set { SetValue("LimitFramerate", value); }
		}

		public int UpdatesPerSecond
		{
			get { return GetValue("UpdatesPerSecond", DefaultUpdatesPerSecond); }
			set { SetValue("UpdatesPerSecond", value); }
		}

		public const int DefaultUpdatesPerSecond = 20;

		public int RapidUpdatesPerSecond
		{
			get { return GetValue("RapidUpdatesPerSecond", 60); }
			set { SetValue("RapidUpdatesPerSecond", value); }
		}

		public ProfilingMode ProfilingModes
		{
			get { return GetValue("ProfilingModes", ProfilingMode.None); }
			set { SetValue("ProfilingModes", value); }
		}

		public string OnlineServiceIp
		{
			get { return GetValue("ContentServerIp", "deltaengine.net"); }
			set { SetValue("ContentServerIp", value); }
		}

		public int OnlineServicePort
		{
			get { return GetValue("ContentServerPort", 800); }
			set { SetValue("ContentServerPort", value); }
		}

		public bool UseVSync
		{
			get { return LimitFramerate > 0 && LimitFramerate <= 120; }
		}

		public bool UseOnlineLogging
		{
			get { return GetValue("UseOnlineLogging", true); }
			set { SetValue("UseOnlineLogging", value); }
		}
	}
}