using System;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Provides total run time and delta time for the current frame in seconds. Only used for
	/// profiling, debugging and fps display. All game logic uses Time.Delta, Time.CheckEvery, etc.
	/// </summary>
	public abstract class GlobalTime : IDisposable
	{
		/// <summary>
		/// StopwatchTime by default, easy to change (tests use MockTime, resolvers restart time).
		/// </summary>
		public static GlobalTime Current
		{
			get { return currentGlobalTime ?? (currentGlobalTime = new StopwatchTime()); }
			private set { currentGlobalTime = value; }
		}
		private static GlobalTime currentGlobalTime;

		protected GlobalTime()
		{
			Current = this;
			SetFpsTo60InitiallyAndSetUsefulInitialValues();
		}

		public void Dispose()
		{
			Current = null;
		}

		private void SetFpsTo60InitiallyAndSetUsefulInitialValues()
		{
			Fps = 60;
			framesCounter = 0;
			thisFrameTicks = 0;
			lastFrameTicks = -TicksPerSecond / Fps;
		}

		protected abstract long TicksPerSecond { get; }
		protected abstract long GetTicks();

		public int Fps { get; private set; }
		private int framesCounter;
		private long thisFrameTicks;
		private long lastFrameTicks;

		public long Milliseconds
		{
			get { return thisFrameTicks * 1000 / TicksPerSecond; }
		}

		public void Update()
		{
			lastFrameTicks = thisFrameTicks;
			thisFrameTicks = GetTicks();
			UpdateFpsEverySecond();
		}

		private void UpdateFpsEverySecond()
		{
			framesCounter++;
			if (thisFrameTicks / TicksPerSecond <= lastFrameTicks / TicksPerSecond)
				return;
			Fps = framesCounter;
			framesCounter = 0;
		}

		/// <summary>
		/// Returns an accurate seconds float value for today, would get inaccurate with more days.
		/// </summary>
		[ConsoleCommand("GetSecondsSinceStartToday")]
		public float GetSecondsSinceStartToday()
		{
			long ticksInADay = TicksPerSecond * 60 * 60 * 24;
			long ticksToday = GetTicks() % ticksInADay;
			return ((float)ticksToday / TicksPerSecond);
		}
	}
}