using System.Collections.Generic;
using CreepyTowers.Enemy.Bosses;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Bosses
{
	public class BossTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			camera = new GameCamera(1 / 15.0f);
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			new Player();
		}

		private GameCamera camera;

		[Test]
		public void NotDrawingCreepDoesntCreateCreepModel()
		{
			new Boss(BossType.Cloak, Vector2D.Zero);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Boss>().Count);
		}

		[Test]
		public void DrawingCreepCreatesCreepModel()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero, 180.0f);
			boss.RenderModel();
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Boss>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckScaleUpdatingForBossVillain()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			boss.RenderModel();
			boss.Scale = 3.0f * Vector3D.One;
			Assert.AreEqual(3.0f * Vector3D.One, boss.Model.Scale);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckDirectionOfBossVillainWithoutPath()
		{
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(5.0f);
			Assert.AreEqual(Vector3D.Zero, boss.Direction);
		}

		[Test]
		public void DisplayBossVillainFollowingPreDefinedPath()
		{
			camera.ZoomLevel = 1 / 20.0f;
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			boss.RenderModel();
			boss.FinalTarget = new Vector2D(2, 2);
			boss.Path = new List<Vector2D>
			{
				new Vector2D(4, 0),
				new Vector2D(4, 9),
				new Vector2D(2, 9),
				new Vector2D(2, 2)
			};
		}

		[Test, CloseAfterFirstFrame]
		public void CheckGoldRewardForBoss()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			Assert.AreEqual(100, boss.GetStatValue("Gold"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttack()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			boss.ReceiveAttack(TowerType.Fire, 50.0f);
			Assert.AreEqual(465.0f, boss.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttackWithInactiveBoss()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			boss.IsActive = false;
			boss.ReceiveAttack(TowerType.Fire, 100.0f);
			Assert.IsFalse(boss.IsActive);
			Assert.AreEqual(boss.GetStatValue("Hp"), boss.GetStatBaseValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttackWithDeadBoss()
		{
			var boss = new Boss(BossType.Cloak, Vector2D.Zero);
			boss.AdjustStat(new StatAdjustment("Hp", "", -500));
			boss.ReceiveAttack(TowerType.Fire, 100.0f);
			Assert.AreEqual(0.0f, boss.GetStatValue("Hp"));
		}
	}
}