namespace CreepyTowers.Stats
{
	/// <summary>
	/// This is used to increase or decrease the value of a creep/tower statistic - either by
	/// multiplying the base value or by adding to it
	/// </summary>
	public class Buff
	{
		public Buff(Stat stat, BuffEffect effect)
		{
			Stat = stat;
			Effect = effect;
		}

		public Stat Stat { get; set; }
		public BuffEffect Effect { get; set; }

		public bool IsExpired
		{
			get
			{
				if (Effect.Duration <= 0)
					return false;
				return Elapsed >= Effect.Duration;
			}
		}

		public float Elapsed { get; set; }
	}
}