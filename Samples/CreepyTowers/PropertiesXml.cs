using System.Collections.Generic;
using System.IO;
using CreepyTowers.Enemy.Bosses;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Content.Xml;

namespace CreepyTowers
{
	public abstract class PropertiesXml : XmlContent
	{
		protected PropertiesXml(string contentName)
			: base(contentName)
		{
			agentData = new List<AgentData>();
		}

		protected readonly List<AgentData> agentData;

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			foreach (var boss in Data.GetChildren("Data"))
				agentData.Add(ParseData(boss));
		}

		protected abstract AgentData ParseData(XmlData data);

		public virtual TowerData Get(TowerType type)
		{
			return null;
		}

		public virtual CreepData Get(CreepType type)
		{
			return null;
		}

		public virtual BossData Get(BossType type)
		{
			return null;
		}
	}
}