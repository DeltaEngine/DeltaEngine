using System;
using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Derived classes implement system profiling - ie. RAM/CPU/etc. used over time.
	/// </summary>
	public interface SystemProfilingProvider
	{
		bool IsActive { get; set; }
		int MaximumPollsPerSecond { get; set; }
		void Log(ProfilingMode profilingMode, SystemInformation systemInformation);
		SystemProfilerSection GetProfilingResults(ProfilingMode profilingMode);
		event Action Updated;
	}
}