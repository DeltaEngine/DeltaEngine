namespace DeltaEngine.Entities
{
	/// <summary>
	/// Easily access the current update time step delta, which defaults to 0.05 (20 updates/sec).
	/// </summary>
	public abstract class Time
	{
		public static float Delta { get; internal set; }
		public static float RapidUpdateDelta { get; internal set; }
		public static float Total { get; internal set; }
		public static float SpeedFactor = 1.0f;

		/// <summary>
		/// Allows to check in huge intervals. Should only be used with interval values above Delta,
		/// otherwise it will always return true as the Total time is only updated once per Update tick.
		/// We subtract Delta since Total already includes this update tick's Delta.
		/// </summary>
		public static bool CheckEvery(float interval)
		{
			return (int)(Total / interval) > (int)((Total - Delta) / interval);
		}

		public static bool IsPaused { get; set; }
	}
}