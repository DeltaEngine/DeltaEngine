using System;
using System.Linq;
using DeltaEngine.Content.Xml;

namespace $safeprojectname$.Towers
{
	/// <summary>
	/// Contains the stats for the Towers
	/// </summary>
	public class TowerPropertiesXml : PropertiesXml
	{
		public TowerPropertiesXml(string contentName)
			: base(contentName) {}

		protected override AgentData ParseData(XmlData tower)
		{
			return
				new TowerData((TowerType)Enum.Parse(typeof(TowerType), tower.GetAttributeValue("Type")),
					tower.GetAttributeValue("Name"),
					(AttackType)Enum.Parse(typeof(AttackType), tower.GetAttributeValue("AttackType")),
					tower.GetAttributeValue("Range", 0.0f), tower.GetAttributeValue("AttackFrequency", 0.0f),
					tower.GetAttributeValue("AttackDamage", 0.0f), tower.GetAttributeValue("Cost", 0));
		}

		public override TowerData Get(TowerType type)
		{
			foreach (TowerData d in agentData.Where(d => (TowerType)d.Type == type))
				return d;
			var newTowerData = DefaultTowerPropertiesForTesting.DefaultTowerValuesForTesting(type);
			agentData.Add(newTowerData);
			return newTowerData;
		}
	}
}