using System.Diagnostics;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides ticks for the Time class via the System.Diagnostics.Stopwatch class. This class is
	/// usually the fallback if nothing else has been registered for Time.Current.
	/// </summary>
	public class StopwatchTime : GlobalTime
	{
		private readonly Stopwatch timer = Stopwatch.StartNew();

		protected override long GetTicks()
		{
			return timer.ElapsedTicks;
		}

		protected override long TicksPerSecond
		{
			get { return Stopwatch.Frequency; }
		}
	}
}