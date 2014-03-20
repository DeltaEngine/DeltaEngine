using System;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Derived classes implement profiling of code time - ie. how long a section of code
	/// takes to run.
	/// </summary>
	public interface CodeProfilingProvider
	{
		void BeginFrame();
		void Start(ProfilingMode profilingMode, string sectionName);
		void Stop(ProfilingMode profilingMode, string sectionName);
		void EndFrame();
		event Action Updated;
		CodeProfilingResults GetProfilingResults(ProfilingMode profilingMode);
		string GetProfilingResultsSummary(ProfilingMode profilingMode);
		float ResetInterval { get; set; }
		int MaximumPollsPerSecond { get; set; }
		bool IsActive { get; set; }
	}
}