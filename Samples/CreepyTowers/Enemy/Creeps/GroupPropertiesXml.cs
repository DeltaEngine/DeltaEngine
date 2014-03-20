using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CreepyTowers.Content;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;

namespace CreepyTowers.Enemy.Creeps
{
	/// <summary>
	/// Contains the definitions for groups of creeps
	/// </summary>
	public class GroupPropertiesXml : XmlContent
	{
		public GroupPropertiesXml(string contentName)
			: base(contentName) {}

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			foreach (XmlData group in Data.GetChildren("Group"))
			{
				var data = ParseGroupData(group);
				var groupName = (CreepGroups)Enum.Parse(typeof(CreepGroups), data.Name);
				groupData[groupName] = data;
			}
		}

		private readonly Dictionary<CreepGroups, GroupData> groupData =
			new Dictionary<CreepGroups, GroupData>();

		private static GroupData ParseGroupData(XmlData group)
		{
			return new GroupData(group.GetAttributeValue("Name"),
				group.GetAttributeValue("CreepTypeList").SplitAndTrim(',').ToList(),
				group.GetAttributeValue("SpawnIntervalList", 0.0f));
		}

		public GroupData Get(CreepGroups groupName)
		{
			if (groupData.Any(data => data.Key == groupName))
				return groupData[groupName];
			var newGroupData = DefaultGroupDataForTesting.GetDefaultGroupData(groupName);
			groupData.Add(groupName, newGroupData);
			return groupData[groupName];
		}
	}
}