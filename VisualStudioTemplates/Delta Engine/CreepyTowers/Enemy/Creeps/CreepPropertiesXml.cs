using System;
using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Towers;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;

namespace $safeprojectname$.Enemy.Creeps
{
	/// <summary>
	/// Contains the stats for the Creeps
	/// </summary> 
	public class CreepPropertiesXml : PropertiesXml
	{
		public CreepPropertiesXml(string contentName)
			: base(contentName) {}

		private readonly List<CreepData> creepData = new List<CreepData>();

		protected override AgentData ParseData(XmlData creep)
		{
			return
				new CreepData((CreepType)Enum.Parse(typeof(CreepType), creep.GetAttributeValue("Type")),
					creep.GetAttributeValue("Name"), creep.GetAttributeValue("MaxHp", 0.0f),
					creep.GetAttributeValue("Speed", 0.0f), creep.GetAttributeValue("Resistance", 0.0f),
					creep.GetAttributeValue("Gold", 0), ParseTypeDamageModifier(creep));
		}

		private static Dictionary<TowerType, float> ParseTypeDamageModifier(XmlData creep)
		{
			var typeDamageModifier = new Dictionary<TowerType, float>();
			foreach (var attribute in creep.GetChild("Modifiers").Attributes)
				typeDamageModifier.Add((TowerType)Enum.Parse(typeof(TowerType), attribute.Name),
					attribute.Value.Convert<float>());
			return typeDamageModifier;
		}

		public override CreepData Get(CreepType type)
		{
			foreach (CreepData d in agentData.Where(d => (CreepType)d.Type == type))
				return d;
			var newCreepData = DefaultCreepPropertiesForTesting.GetDefaultCreepData(type);
			creepData.Add(newCreepData);
			return newCreepData;
		}
	}
}