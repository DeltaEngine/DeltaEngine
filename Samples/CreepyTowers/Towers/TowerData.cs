namespace CreepyTowers.Towers
{
	/// <summary>
	/// Initial data describing a tower, has no current in game values.
	/// </summary>
	public class TowerData : AgentData
	{
		public TowerData(TowerType type, string name, AttackType attackType, float range,
			float frequency, float basePower, int cost)
			: base(name)
		{
			Type = type;
			AttackType = attackType;
			Range = range;
			AttackFrequency = frequency;
			BasePower = basePower;
			Cost = cost;
		}

		public AttackType AttackType { get; private set; }
		public float Range { get; private set; }
		public float AttackFrequency { get; private set; }
		public float BasePower { get; private set; }
		public int Cost { get; private set; }
	}
}