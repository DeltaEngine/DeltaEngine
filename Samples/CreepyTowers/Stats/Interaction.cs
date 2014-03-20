namespace CreepyTowers.Stats
{
	/// <summary>
	/// This is created when one thing interacts with another and carries information on how much to 
	/// adjust its stats and/or buffs (eg. a tower's bullet hitting a creep)
	/// </summary>
	public class Interaction
	{
		public Interaction(Agent source, Agent target, StatAdjustment adjustment, BuffEffect? effect = null)
		{
			Source = source;
			Target = target;
			this.adjustment = adjustment;
			this.effect = effect;
		}

		public Agent Source { get; private set; }
		public Agent Target { get; private set; }
		private readonly StatAdjustment adjustment;
		private readonly BuffEffect? effect;

		public void Apply()
		{
			Target.AdjustStat(adjustment);
			if (effect != null)
				Target.ApplyBuff((BuffEffect)effect);
		}
	}
}