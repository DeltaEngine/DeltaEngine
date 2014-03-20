using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class GroupPropertiesXmlTests : TestWithCreepyTowersMockContentLoaderOrVisually
	{
		[SetUp]
		public void Initialize()
		{
			groupProperties = ContentLoader.Load<GroupPropertiesXml>(Xml.GroupProperties.ToString());
		}

		private GroupPropertiesXml groupProperties;

		[Test, CloseAfterFirstFrame]
		public void TestGroupPropertiesXml()
		{
			var groupData = groupProperties.Get(CreepGroups.Paper2);
			Assert.AreEqual(0.5f, groupData.CreepSpawnInterval);
			Assert.AreEqual("Paper2", groupData.Name);
			var list = new List<CreepType> { CreepType.Paper, CreepType.Paper };
			Assert.AreEqual(list, groupData.CreepList);
		}

		//[Test, CloseAfterFirstFrame]
		//public void TestGroupPropertiesXmlWithUnknownType()
		//{
		//	var groupData = groupProperties.Get(CreepGroups.Mix);
		//	Assert.AreEqual(0.75f, groupData.CreepSpawnInterval);
		//	Assert.AreEqual("Mix", groupData.Name);
		//	var list = new List<CreepType>
		//	{
		//		CreepType.Plastic,
		//		CreepType.Paper,
		//		CreepType.Wood,
		//		CreepType.Wood,
		//		CreepType.Cloth,
		//		CreepType.Plastic
		//	};
		//	Assert.AreEqual(list, groupData.CreepList);
		//}
	}
}