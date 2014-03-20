using System.Collections.Generic;
using CreepyTowers.Content;

namespace $safeprojectname$.Enemy.Creeps
{
	public class DefaultGroupDataForTesting
	{
		public static GroupData GetDefaultGroupData(CreepGroups groupName)
		{
			List<string> creepNamesInGroup;
			switch (groupName)
			{
			case CreepGroups.Paper2:
				creepNamesInGroup = new List<string> { "Paper", "Paper" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.Paper3:
				creepNamesInGroup = new List<string> { "Paper", "Paper", "Paper" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.Cloth2:
				creepNamesInGroup = new List<string> { "Cloth", "Cloth" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.Cloth3:
				creepNamesInGroup = new List<string> { "Cloth", "Cloth", "Cloth" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.Plastic2:
				creepNamesInGroup = new List<string> { "Plastic", "Plastic" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.Plastic3:
				creepNamesInGroup = new List<string> { "Plastic", "Plastic", "Plastic" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.GroupPpCl:
				creepNamesInGroup = new List<string> { "Paper", "Cloth" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.GroupPpPl:
				creepNamesInGroup = new List<string> { "Paper", "Plastic" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;

			case CreepGroups.GroupClPl:
				creepNamesInGroup = new List<string> { "Cloth", "Plastic" };
				defaultGroupData = new GroupData(groupName.ToString(), creepNamesInGroup, 0.5f);
				break;
			}
			return defaultGroupData;
		}

		private static GroupData defaultGroupData;
	}
}