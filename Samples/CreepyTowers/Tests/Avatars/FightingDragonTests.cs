using CreepyTowers.Avatars;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Avatars
{
	public class FightingDragonTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void SetUp()
		{
			player = new Player();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		private Player player;

		[Test]
		public void WithoutFightingDragonTowerHasDefaultRange()
		{
			player.UnlockAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			var tower = new Tower(TowerType.Fire, Vector3D.Zero);
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var defaultRange = towerProperties.Get(TowerType.Fire).Range;
			Assert.AreEqual(defaultRange, tower.GetStatValue("Range"));
		}

		[Test]
		public void WithFightingDragonTowerHasExtendedRange()
		{
			var tower = new Tower(TowerType.Fire, Vector3D.Zero);
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var defaultRange = towerProperties.Get(TowerType.Fire).Range;
			Assert.AreEqual(defaultRange * 1.25f, tower.GetStatValue("Range"));
		}

		[Test]
		public void PerformBreathOfFireAttack()
		{
			var creepOutsideRange = new Creep(CreepType.Paper, new Vector2D(3, 3));
			var creepInsideRange = new Creep(CreepType.Paper, new Vector2D(2, 2));
			player.Avatar.PerformAttack(AvatarAttack.DragonBreathOfFire, new Vector2D(1, 1));
			Assert.AreEqual(1, creepOutsideRange.GetStatPercentage("Hp"));
			AdvanceTimeAndUpdateEntities(Dragon.BreathOfFireTimeTillImpact + 0.15f);
			Assert.Less(creepInsideRange.GetStatPercentage("Hp"), 1);
		}

		[Test]
		public void PerformAuraCannonAttack()
		{
			var creepOutsideRange = new Creep(CreepType.Paper, new Vector2D(1, 4));
			var creepInsideRange = new Creep(CreepType.Paper, new Vector2D(2, 2));
			player.Avatar.PerformAttack(AvatarAttack.DragonAuraCannon, new Vector2D(1, 2));
			Assert.AreEqual(0.933f, creepOutsideRange.GetStatPercentage("Hp"), 0.001f);
			Assert.Less(creepInsideRange.GetStatPercentage("Hp"), 1);
			AdvanceTimeAndUpdateEntities(1.05f);
		}
	}
}