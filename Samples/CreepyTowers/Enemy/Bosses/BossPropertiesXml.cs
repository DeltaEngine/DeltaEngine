using System;
using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Towers;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;

namespace CreepyTowers.Enemy.Bosses
{
	public class BossPropertiesXml : PropertiesXml
	{
		protected BossPropertiesXml(string contentName)
			: base(contentName) {}

		protected override AgentData ParseData(XmlData boss)
		{
			return new BossData((BossType)Enum.Parse(typeof(BossType), boss.GetAttributeValue("Type")),
				boss.GetAttributeValue("Name"), boss.GetAttributeValue("MaxHp", 0.0f),
				boss.GetAttributeValue("Speed", 0.0f), boss.GetAttributeValue("Resistance", 0.0f),
				boss.GetAttributeValue("Gold", 0), ParseTypeDamageModifier(boss));
		}

		private static Dictionary<TowerType, float> ParseTypeDamageModifier(XmlData boss)
		{
			return
				boss.GetChild("Modifiers").Attributes.ToDictionary(
					attribute => (TowerType)Enum.Parse(typeof(TowerType), attribute.Name),
					attribute => attribute.Value.Convert<float>());
		}

		public override BossData Get(BossType type)
		{
			foreach (BossData d in agentData.Where(d => (BossType)d.Type == type))
				return d;
			var newBossData = DefaultBossPropertiesForTesting.GetDefaultBossData(type);
			agentData.Add(newBossData);
			return newBossData;
		}
	}
}