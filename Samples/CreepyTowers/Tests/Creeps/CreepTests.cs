using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CreepTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			camera = new GameCamera(1 / 5.0f);
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			new Player();
		}

		private GameCamera camera;

		[Test]
		public void NotDrawingCreepDoesntCreateCreepModel()
		{
			new Creep(CreepType.Cloth, Vector3D.Zero);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test]
		public void DrawingCreepCreatesCreepModel()
		{
			var creep = new Creep(CreepType.Cloth, Vector3D.Zero);
			creep.RenderModel();
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test]
		public void DisplayClothCreep()
		{
			var creep = new Creep(CreepType.Cloth, Vector3D.Zero, 180.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplayGlassCreep()
		{
			var creep = new Creep(CreepType.Glass, Vector3D.Zero, 150.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplayPlasticCreep()
		{
			var creep = new Creep(CreepType.Plastic, Vector3D.Zero, 180.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplayIronCreep()
		{
			var creep = new Creep(CreepType.Iron, Vector3D.Zero, 210.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplaySandCreep()
		{
			var creep = new Creep(CreepType.Sand, Vector3D.Zero, 180.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplayPaperCreep()
		{
			var creep = new Creep(CreepType.Paper, Vector3D.Zero, 180.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test]
		public void DisplayWoodCreep()
		{
			var creep = new Creep(CreepType.Wood, Vector3D.Zero, 180.0f);
			creep.HealthBar.IsVisible = false;
			creep.RenderModel();
		}

		[Test, CloseAfterFirstFrame]
		public void CheckScaleUpdatingForCreeps()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.RenderModel();
			creep.Scale = 3.0f * Vector3D.One;
			Assert.AreEqual(3.0f * Vector3D.One, creep.Model.Scale);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckDirectionOfCreepWithoutPath()
		{
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(5.0f);
			Assert.AreEqual(Vector3D.Zero, creep.Direction);
		}

		[Test]
		public void DisplayCreepFollowingPreDefinedPath()
		{
			camera.ZoomLevel = 1 / 40.0f;
			var grid = new Grid3D(new Size(10, 10));
			grid.RenderLayer = -10;
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.RenderModel();
			creep.FinalTarget = new Vector2D(2, 2);
			creep.Path = new List<Vector2D>
			{
				new Vector2D(4, 0),
				new Vector2D(4, 9),
				new Vector2D(2, 9),
				new Vector2D(2, 2)
			};
		}

		[Test, CloseAfterFirstFrame]
		public void CheckGoldRewardForCreeps()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			Assert.AreEqual(13, creep.GetStatValue("Gold"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttack()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			Assert.AreEqual(100.0f, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttackWithInactiveCreep()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.IsActive = false;
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			Assert.IsFalse(creep.IsActive);
			Assert.AreEqual(creep.GetStatValue("Hp"), creep.GetStatBaseValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckReceiveAttackWithDeadCreep()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.AdjustStat(new StatAdjustment("Hp", "", -195));
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			Assert.AreEqual(0.0f, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckUpdateWithDeadCreep()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			creep.AdjustStat(new StatAdjustment("Hp", "", -185));
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			creep.Update();
			Assert.AreEqual(0.0f, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckStateBurn()
		{
			var creep = new Creep(CreepType.Plastic, Vector2D.Zero);
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			AdvanceTimeAndUpdateEntities();
			creep.Update();
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			AdvanceTimeAndUpdateEntities();
			creep.Update();
			Assert.Less(creep.GetStatValue("Hp"), 100.0f);
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void CheckStateBurst()
		{
			var creep = new Creep(CreepType.Paper, Vector2D.Zero);
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			AdvanceTimeAndUpdateEntities(1.0f);
			creep.Update();
			creep.ReceiveAttack(TowerType.Fire, 10.0f);
			AdvanceTimeAndUpdateEntities(2.1f);
			creep.Update();
			Assert.Less(creep.GetStatValue("Hp"), 90.0f);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckShatter()
		{
			var creep = new Creep(CreepType.Glass, Vector2D.Zero);
			var nearCreep = new Creep(CreepType.Wood, Vector2D.UnitX);
			creep.Shatter();
			Assert.AreEqual(45.0f, nearCreep.GetStatValue("Hp"));
		}
	}
}