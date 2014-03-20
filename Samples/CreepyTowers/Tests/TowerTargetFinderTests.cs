using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Particles;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class TowerTargetFinderTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void InitializeMockUpdater()
		{
			new TowerTargetFinder();
			new Player();
		}

		private static Creep CreateCreepOfType(CreepType type, Vector3D position)
		{
			return new Creep(type, position);
		}

		[Test, CloseAfterFirstFrame]
		public void InAbsenceOfCreepsTheTowerDoesNotFire()
		{
			var tower = new Tower(TowerType.Fire, Vector2D.Zero);
			AdvanceTimeTillTowerAttacks(tower);
			Assert.IsFalse(tower.IsOnCooldown);
		}

		private void AdvanceTimeTillTowerAttacks(Tower tower)
		{
			var towerCooldown = 1 / tower.GetStatValue("AttackFrequency");
			AdvanceTimeAndUpdateEntities(towerCooldown + 0.1f);
		}

		[Test, CloseAfterFirstFrame]
		public void TowerDoesNotAttackIfStillInCooldown()
		{
			var tower = new Tower(TowerType.Water, Vector2D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, 2.0f * Vector3D.UnitY);
			tower.RenderModel();
			AdvanceToJustBeforeTowerAttack(tower);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.AreEqual(1, creep.GetStatPercentage("Hp"));
		}

		private void AdvanceToJustBeforeTowerAttack(Tower tower)
		{
			var towerCooldown = 1 / tower.GetStatValue("AttackFrequency");
			AdvanceTimeAndUpdateEntities(towerCooldown - 0.1f);
		}

		public void TowerDoesNotAttackIfTooFarFromCreep()
		{
			var tower = new Tower(TowerType.Water, Vector2D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, 2.0f * Vector3D.UnitY);
			creep.Position = new Vector3D(100, 100, 0);
			AdvanceTimeTillTowerAttacks(tower);
			Assert.IsFalse(tower.IsOnCooldown);
			Assert.AreEqual(1, creep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void InactiveTowerDoesNotFire()
		{
			var tower = new Tower(TowerType.Water, Vector2D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			tower.IsActive = false;
			AdvanceTimeTillTowerAttacks(tower);
			Assert.AreEqual(1, creep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void TowerDoesNotAttackInactiveCreep()
		{
			var tower = new Tower(TowerType.Water, Vector2D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			creep.IsActive = false;
			AdvanceTimeTillTowerAttacks(tower);
			Assert.AreEqual(1, creep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void DirectShotTowerFiresAtNearestCreep()
		{
			var tower = new Tower(TowerType.Water, Vector2D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			tower.RenderModel();
			var furtherCreep = CreateCreepOfType(CreepType.Cloth, new Vector2D(2.75f, 2.75f));
			AdvanceTimeTillTowerAttacks(tower);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.LessOrEqual(0, EntitiesRunner.Current.GetEntitiesOfType<ParticleEmitter>().Count);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
			Assert.AreEqual(1, furtherCreep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void RadiusAttackExcludesCreepsOutsideRange()
		{
			var tower = new Tower(TowerType.Slice, Vector3D.Zero);
			tower.RenderModel();
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			Assert.AreEqual(AttackType.Cone, tower.AttackType);
			var farCreep = CreateCreepOfType(CreepType.Cloth, new Vector3D(100, 100, 0));
			AdvanceTimeTillTowerAttacks(tower);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
			Assert.AreEqual(1, farCreep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void RadiusAttackIncludesCreepsWithinRange()
		{
			var tower = new Tower(TowerType.Fire, Vector3D.Zero);
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			var nearCreep = new Creep(CreepType.Cloth, new Vector2D(0.0f, 2.0f));
			AdvanceTimeTillTowerAttacks(tower);
			Assert.AreEqual(AttackType.Circle, tower.AttackType);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
			Assert.Less(nearCreep.GetStatPercentage("Hp"), 1);
		}

		[Test, CloseAfterFirstFrame]
		public void ConeAttackExcludesCreepsOutsideAngle()
		{
			var tower = new Tower(TowerType.Slice, Vector3D.Zero);
			tower.RenderModel();
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			var outsideCreep = CreateCreepOfType(CreepType.Cloth, new Vector2D(2.75f, 1.0f));
			AdvanceTimeTillTowerAttacks(tower);
			Assert.AreEqual(AttackType.Cone, tower.AttackType);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
			Assert.AreEqual(1, outsideCreep.GetStatPercentage("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void ConeAttackIncludesCreepsWithinAngle()
		{
			var tower = new Tower(TowerType.Slice, Vector3D.Zero);
			tower.RenderModel();
			var creep = CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			var insideCreep = CreateCreepOfType(CreepType.Cloth, new Vector2D(0.0f, 1.5f));
			AdvanceTimeTillTowerAttacks(tower);
			Assert.AreEqual(AttackType.Cone, tower.AttackType);
			Assert.IsTrue(tower.IsOnCooldown);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
			Assert.Less(insideCreep.GetStatPercentage("Hp"), 1);
		}

		[Test]
		public void SuccessfulAttackProducesHitEffect()
		{
			var tower = new Tower(TowerType.Fire, new Vector3D(2, 2, 0));
			CreateCreepOfType(CreepType.Cloth, Vector3D.UnitY);
			AdvanceTimeTillTowerAttacks(tower);
		}
	}
}