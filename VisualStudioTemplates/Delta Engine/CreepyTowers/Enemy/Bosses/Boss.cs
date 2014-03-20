using CreepyTowers.Content;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace $safeprojectname$.Enemy.Bosses
{
	public sealed class Boss : Enemy
	{
		public Boss(BossType type, Vector2D position, float rotationZ = 0.0f)
			: base(position, rotationZ)
		{
			Type = type;
			CreateStats(type, ContentLoader.Load<BossPropertiesXml>(Xml.BossProperties.ToString()));
		}

		public BossType Type { get; private set; }

		private void CreateStats(BossType type, BossPropertiesXml properties)
		{
			BossData bossData = properties.Get(type);
			Name = bossData.Name;
			CreateStat("Hp", bossData.MaxHp);
			CreateStat("Speed", bossData.Speed);
			CreateStat("Resistance", bossData.Resistance);
			CreateStat("Gold", bossData.GoldReward);
			if (Player.Current != null)
				ApplyBuff(new BuffEffect(Player.Current.Avatar.GetType().Name + "GoldMultiplier"));

			State = new BossState();
			foreach (var modifier in bossData.TypeDamageModifier)
				State.SetVulnerabilityWithValue(modifier.Key, modifier.Value);
		}

		public BossState State { get; private set; }

		public override void UpdateDamageState(TowerType damageType)
		{
			StateChanger.CheckBossState(damageType, this);
		}

		public override void HasReachedExit() {}
		protected override void RestartStatsAndState(float percentage, AgentData data) {}
	}
}