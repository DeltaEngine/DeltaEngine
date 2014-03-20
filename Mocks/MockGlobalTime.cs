using DeltaEngine.Core;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Increments the number of ticks that have passed every time it is polled.
	/// </summary>
	public class MockGlobalTime : GlobalTime
	{
		protected override long GetTicks()
		{
			return ++ticks;
		}

		private int ticks;

		protected override long TicksPerSecond
		{
			get { return UpdatePerSecond; }
		}

		public const int UpdatePerSecond = 20;
	}
}
