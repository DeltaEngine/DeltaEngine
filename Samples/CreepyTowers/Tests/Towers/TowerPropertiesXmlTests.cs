using CreepyTowers.Content;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Towers
{
	public class TowerPropertiesXmlTests : TestWithCreepyTowersMockContentLoaderOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void TestTowerPropertiesXml()
		{
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var towerData = towerProperties.Get(TowerType.Acid);
			Assert.AreEqual(1.0f, towerData.AttackFrequency);
			Assert.AreEqual(4.0f, towerData.Range);
			Assert.AreEqual(35.0f, towerData.BasePower);
			Assert.AreEqual(230, towerData.Cost);
			Assert.AreEqual(AttackType.DirectShot, towerData.AttackType);
		}
	}
}