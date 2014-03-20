using System;

namespace $safeprojectname$
{
	/// <summary>
	/// Temporary class until CreepData and TowerData can be merged into a string-based generic class
	/// </summary>
	public abstract class AgentData
	{
		protected AgentData(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
		public Enum Type { get; set; }
	}
}