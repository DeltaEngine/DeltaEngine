using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class GroupTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CheckGroupXmlData()
		{
			var groupProperties = ContentLoader.Load<GroupPropertiesXml>("GroupProperties");
			Assert.AreEqual(2, groupProperties.Get(CreepGroups.Paper2).CreepList.Count);
			Assert.AreEqual(3, groupProperties.Get(CreepGroups.Cloth3).CreepList.Count);
		}
	}
}