using System;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Allows delegates to be run millions of times to compare different implementations.
	/// </summary>
	public class DelegateProfiler
	{
		public DelegateProfiler(Action testDelegate, int iterations = 1000)
		{
			testDelegate();
			float start = GlobalTime.Current.GetSecondsSinceStartToday();
			for (int i = 0; i < iterations; i++)
				testDelegate();

			float elapsed = GlobalTime.Current.GetSecondsSinceStartToday() - start;
			TotalDurationInMilliseconds = (int)(elapsed * 1000);
			AverageDurationInNanoseconds = (int)(0.5f + elapsed * 1000000.0 / iterations);
			AverageDurationInPicoseconds = (int)(0.5f + elapsed * 1000000000.0 / iterations);
		}

		public int TotalDurationInMilliseconds { get; private set; }
		public int AverageDurationInNanoseconds { get; private set; }
		public int AverageDurationInPicoseconds { get; private set; }
	}
}