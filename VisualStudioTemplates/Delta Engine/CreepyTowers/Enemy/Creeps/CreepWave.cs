using System;
using System.Collections.Generic;
using CreepyTowers.Content;
using DeltaEngine.GameLogic;

namespace $safeprojectname$.Enemy.Creeps
{
	public class CreepWave : Wave
	{
		public CreepWave(float waitTime, float spawnInterval, string thingsToSpawn = "",
			string shortName = "", float maxTime = 0.0f, int maxSpawnItems = 1)
			: base(waitTime, spawnInterval, thingsToSpawn, shortName, maxTime, maxSpawnItems)
		{
			CreepsAndGroupsList = new List<Object>();
			TotalCreepsInWave = 0;
			GetCreepSpawnList();
		}

		public CreepWave(Wave wave)
			: this(
				wave.WaitTime, wave.SpawnInterval, wave.ToString(), wave.ShortName,
				wave.MaxTime, wave.MaxSpawnItems) {}

		public List<Object> CreepsAndGroupsList { get; private set; }
		public int TotalCreepsInWave { get; private set; }

		private void GetCreepSpawnList()
		{
			foreach (var name in SpawnTypeList)
				if (creepList.Contains(name))
				{
					CreepsAndGroupsList.Add(GroupData.FindAppropriateCreepType(name));
					TotalCreepsInWave++;
				}
				else if (groupList.Contains(name))
				{
					var groupName = (CreepGroups)Enum.Parse(typeof(CreepGroups), name);
					var groupData = DefaultGroupDataForTesting.GetDefaultGroupData(groupName);
					CreepsAndGroupsList.Add(groupData);
					TotalCreepsInWave += groupData.CreepList.Count;
				}
		}

		private readonly List<string> creepList = new List<string>
		{
			"Cloth",
			"Iron",
			"Paper",
			"Wood",
			"Glass",
			"Plastic",
			"Sand"
		};

		private readonly List<string> groupList = new List<string>
		{
			"Paper2",
			"Paper3",
			"Cloth2",
			"Cloth3",
			"Plastic2",
			"Plastic3",
			"GroupPpPl",
			"GroupPpCl",
			"GroupClPl"
		};
	}
}