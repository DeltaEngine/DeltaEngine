using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// When this is polled, it logs whatever system information the Settings.ProfilingMode 
	/// indicates.
	/// </summary>
	public class SystemProfiler : SystemProfilingProvider
	{
		static SystemProfiler()
		{
			Current = new SystemProfiler { IsActive = false };
		}

		public static SystemProfilingProvider Current { get; set; }
		public bool IsActive { get; set; }

		public SystemProfiler(int maximumPollsPerSecond = 10)
		{
			MaximumPollsPerSecond = maximumPollsPerSecond;
			CreateSections();
			IsActive = true;
		}

		public int MaximumPollsPerSecond
		{
			get { return pollingInterval == 0.0f ? 0 : (int)(0.5f + 1.0f / pollingInterval); }
			set { pollingInterval = value == 0 ? 0.0f : 1.0f / value; }
		}

		private float pollingInterval;

		private void CreateSections()
		{
			Sections = new SystemProfilerSection[CategoryCount];
			for (int i = 0; i < CategoryCount; i++)
				Sections[i] = new SystemProfilerSection(IndexToProfilingMode(i).ToString());
		}

		private static readonly int CategoryCount = Enum.GetNames(typeof(ProfilingMode)).Length;
		internal SystemProfilerSection[] Sections;

		private static ProfilingMode IndexToProfilingMode(int index)
		{
			return (ProfilingMode)(1 >> index);
		}

		public void Log(ProfilingMode profilingMode, SystemInformation systemInformation)
		{
			float time = GlobalTime.Current.GetSecondsSinceStartToday();
			if (!IsActive || time - lastTimeProfiled < pollingInterval)
				return;
			lastTimeProfiled = time;
			LogProfilingModeValues(profilingMode, systemInformation);
			if (Updated != null)
				Updated();
		}

		private float lastTimeProfiled = -1.0f;
		public event Action Updated;

		private void LogProfilingModeValues(ProfilingMode profilingMode,
			SystemInformation systemInformation)
		{
			if (profilingMode.HasFlag(ProfilingMode.Fps))
				GetProfilingResults(ProfilingMode.Fps).Log(GlobalTime.Current.Fps);
			if (profilingMode.HasFlag(ProfilingMode.AvailableRam))
				GetProfilingResults(ProfilingMode.AvailableRam).Log(systemInformation.AvailableRam);
		}

		public SystemProfilerSection GetProfilingResults(ProfilingMode profilingMode)
		{
			return Sections[ProfilingModeToIndex(profilingMode)];
		}

		private static int ProfilingModeToIndex(ProfilingMode profilingMode)
		{
			var profilingModeFlags = (int)profilingMode;
			int profilingModeInt = -1;
			while (profilingModeFlags > 0)
			{
				profilingModeFlags = profilingModeFlags >> 1;
				profilingModeInt++;
			}
			return profilingModeInt;
		}
	}
}