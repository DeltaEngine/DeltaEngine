using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Combination of profiling modes that can be turned on via Settings. Normally all modes are off.
	/// Implementation can be found in the DeltaEngine.Profiling namespace.
	/// </summary>
	[Flags]
	public enum ProfilingMode
	{
		None,
		Fps,
		Engine,
		Application,
		Rendering,
		Shader,
		UI,
		Physics,
		AI,
		Text,
		AvailableRam
	}
}