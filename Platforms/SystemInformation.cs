using System;
using System.Globalization;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Platforms
{
	/// <summary>
	/// Base system information class that is implemented in each platform module.
	/// It provides system information that does not change over the course of a running 
	/// application or does not need updates.
	/// </summary>
	public abstract class SystemInformation
	{
		public abstract float AvailableRam { get; }
		public abstract int CoreCount { get; }
		public abstract string CpuName { get; }
		public abstract float CpuSpeed { get; }
		public abstract float[] CpuUsage { get; }
		public abstract string GpuName { get; }
		public abstract bool IsConsole { get; }
		public abstract bool IsMobileDevice { get; }
		public abstract bool IsTablet { get; }

		public string Language
		{
			get
			{
				string englishName = CurrentCulture.EnglishName;
				if (englishName.Contains(" "))
					englishName = englishName.Substring(0, englishName.IndexOf(' '));
				return englishName;
			}
		}

		protected CultureInfo CurrentCulture
		{
			get { return CultureInfo.CurrentCulture; }
		}

		public abstract string MachineName { get; }
		public abstract float MaxRam { get; }
		public abstract Size MaxResolution { get; }
		public abstract NetworkState NetworkState { get; }
		public abstract string PlatformName { get; }
		public abstract Version PlatformVersion { get; }
		public abstract bool SoundCardAvailable { get; }
		public abstract float UsedRam { get; }
		public abstract string Username { get; }
		public abstract Version Version { get; }
	}
}