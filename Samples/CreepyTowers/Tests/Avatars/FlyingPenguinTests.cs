using CreepyTowers.Avatars;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace CreepyTowers.Tests.Avatars
{
	public class FlyingPenguinTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			player = new Player("", CreepyTowers.Content.Avatars.Penguin);
		}

		private Player player;

		[Test]
		public void WithoutFlyingPenguinTowerHasDefaultAttackFrequency()
		{
			player.UnlockAvatar(CreepyTowers.Content.Avatars.Dragon);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.Dragon);
			var tower = new Tower(TowerType.Fire, Vector3D.Zero);
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var defaultAttackFrequency = towerProperties.Get(TowerType.Fire).AttackFrequency;
			Assert.AreEqual(defaultAttackFrequency, tower.GetStatValue("AttackFrequency"));
		}

		[Test]
		public void WithFlyingPenguinTowerHasFasterAttacks()
		{
			var tower = new Tower(TowerType.Fire, Vector3D.Zero);
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var defaultAttackFrequency = towerProperties.Get(TowerType.Fire).AttackFrequency;
			Assert.AreEqual(defaultAttackFrequency * 1.3333f, tower.GetStatValue("AttackFrequency"));
		}

		[Test]
		public void PerformBigFireworkAttack()
		{
			var creepOutsideRange = new Creep(CreepType.Paper, new Vector2D(4, 4));
			var creepInsideRange = new Creep(CreepType.Paper, new Vector2D(2, 2));
			player.Avatar.PerformAttack(AvatarAttack.PenguinBigFirework, new Vector2D(1, 1));
			Assert.AreEqual(1, creepOutsideRange.GetStatPercentage("Hp"));
			Assert.Less(creepInsideRange.GetStatPercentage("Hp"), 1);
		}

		[Test]
		public void PerformCarpetBombingAttack()
		{
			new Level(new Size(5, 5));
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.2f, 0.1f, 0.2f, 0.1f, 0.2f, 0.3f }));
			var missedCreep = new Creep(CreepType.Paper, new Vector2D(4, 4));
			var hitCreep = new Creep(CreepType.Paper, new Vector2D(0, 1));
			player.Avatar.PerformAttack(AvatarAttack.PenguinCarpetBombing, Vector2D.Unused);
			Assert.AreEqual(1, missedCreep.GetStatPercentage("Hp"));
			Assert.Less(hitCreep.GetStatPercentage("Hp"), 1);
		}
	}
}