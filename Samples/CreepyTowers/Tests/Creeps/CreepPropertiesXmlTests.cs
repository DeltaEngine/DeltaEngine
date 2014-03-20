using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CreepPropertiesXmlTests : TestWithCreepyTowersMockContentLoaderOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void TestCreepPropertiesXml()
		{
			var creepProperties = ContentLoader.Load<CreepPropertiesXml>(Xml.CreepProperties.ToString());
			var creepData = creepProperties.Get(CreepType.Cloth);
			Assert.AreEqual(CreepType.Cloth, creepData.Type);
			Assert.AreEqual(100, creepData.MaxHp);
			Assert.AreEqual(10, creepData.Resistance);
			Assert.AreEqual(1, creepData.Speed);
			Assert.AreEqual(13, creepData.GoldReward);
		}
	}
}