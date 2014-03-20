using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace CreepyTowers.Tests.Towers
{
	public class TowerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Initialize()
		{
			Command.Register("ViewPanning", new Trigger[] { new MouseDragTrigger(MouseButton.Middle) });
			Command.Register("ViewZooming", new Trigger[] { new MouseZoomTrigger() });
			Command.Register("TurnViewRight", new Trigger[] { new KeyTrigger(Key.E) });
			Command.Register("TurnViewLeft", new Trigger[] { new KeyTrigger(Key.Q) });
			camera = new GameCamera(1 / 15.0f);
			new Player();
		}

		private GameCamera camera;

		[Test, CloseAfterFirstFrame]
		public void NotInitializingTowerDoesntCreateTowerModel()
		{
			new Tower(TowerType.Water, Vector3D.Zero);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Tower>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void InitializingTowerCreatesTowerModel()
		{
			var tower = new Tower(TowerType.Water, Vector3D.Zero);
			tower.RenderModel();
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Model>().Count);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Tower>().Count);
		}

		[Test]
		public void DisplayWaterTower()
		{
			var tower = new Tower(TowerType.Water, Vector3D.Zero, 170.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplayAcidTower()
		{
			var tower = new Tower(TowerType.Acid, Vector3D.Zero, 120.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplayFireTower()
		{
			var tower = new Tower(TowerType.Fire, Vector3D.Zero, 150.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplayIceTower()
		{
			var tower = new Tower(TowerType.Ice, Vector3D.Zero, 160.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplayImpactTower()
		{
			var tower = new Tower(TowerType.Impact, Vector3D.Zero, 120.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplaySliceTower()
		{
			var tower = new Tower(TowerType.Slice, Vector3D.Zero, 120.0f);
			tower.RenderModel();
		}

		[Test]
		public void DisplayAndDisposeTower()
		{
			var tower = new Tower(TowerType.Water, Vector3D.Zero, 170.0f);
			tower.RenderModel();
			tower.Dispose();
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Tower>().Count);
		}

		[Test]
		public void CreateTowerAtClickedPosition()
		{
			var floor = new Plane(Vector3D.UnitY, 0.0f);
			new Command(pos =>
			{ //ncrunch: no coverage start
				var ray = Camera.Current.ScreenPointToRay(ScreenSpace.Current.ToPixelSpace(pos));
				var position = floor.Intersect(ray).Value;
				new Tower(TowerType.Water, position); //ncrunch: no coverage end
			}).Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
		}

		[Test, CloseAfterFirstFrame]
		public void DisposingTowerRemovesTowerEntity()
		{
			var tower = new Tower(TowerType.Water, Vector3D.Zero);
			tower.Dispose();
			Assert.IsFalse(tower.IsActive);
		}

		[Test]
		public void ShowTowerFiringAtCreepAtRegularIntervals()
		{
			camera.ZoomLevel = 1 / 20.0f;
			var tower = new Tower(TowerType.Water, new Vector2D(0, 0));
			var creep = new Creep(CreepType.Cloth, new Vector2D(2, 0));
			tower.RenderModel();
			creep.RenderModel();
			new TowerTargetFinder();
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForCreepUnderAttack()
		{
			var tower = new Tower(TowerType.Slice, new Vector2D(0, 0));
			tower.RenderModel();
			var creep = new Creep(CreepType.Cloth, new Vector2D(1, 0));
			new TowerTargetFinder();
			AdvanceTimeAndUpdateEntities(2.1f);
			Assert.Less(creep.GetStatPercentage("Hp"), 1);
		}

		[Test]
		public void ShowTowerFiringAtAMovingCreep()
		{
			camera.ZoomLevel = 1 / 20.0f;
			var tower = new Tower(TowerType.Water, new Vector2D(0, 0));
			var creep = new Creep(CreepType.Cloth, new Vector2D(1, 3));
			creep.Path = new List<Vector2D> { (new Vector2D(1, -10)) };
			tower.RenderModel();
			creep.RenderModel();
			new TowerTargetFinder();
		}

		[Test]
		public void ShowAllTowerTypesAndEffects()
		{
			var tower = new Tower(TowerType.Acid, Vector3D.Zero);
			tower.RenderModel();
			var creep = new Creep(CreepType.Cloth, new Vector2D(2, 8));
			new Command(() => { tower.FireAtCreep(creep); }).Add(new KeyTrigger(Key.Space));
			new Command(() => { tower.RotationZ += 2; }).Add(new KeyTrigger(Key.A, State.Pressed));
			new Command(() => { tower.RotationZ -= 2; }).Add(new KeyTrigger(Key.D, State.Pressed));
			new Command(() =>
			{ //ncrunch: no coverage start
				tower.Dispose();
				tower = new Tower(CycleTypeUpwards(tower.Type), Vector3D.Zero);
				tower.RenderModel(); //ncrunch: no coverage end
			}).Add(new KeyTrigger(Key.W));
			new Command(() =>
			{ //ncrunch: no coverage start
				tower.Dispose();
				tower = new Tower(CycleTypeDownwards(tower.Type), Vector3D.Zero);
				tower.RenderModel(); //ncrunch: no coverage end
			}).Add(new KeyTrigger(Key.S));
		}

		private static TowerType CycleTypeUpwards(TowerType type)
		{ //ncrunch: no coverage start
			if (type + 1 > TowerType.Water)
				return TowerType.Acid;
			return type + 1;
		} //ncrunch: no coverage end

		private static TowerType CycleTypeDownwards(TowerType type)
		{ //ncrunch: no coverage start
			if (type - 1 < TowerType.Acid)
				return TowerType.Water;
			return type - 1;
		} //ncrunch: no coverage end

		[Test]
		public void ShowWaterTowerWithSunlightEnabled()
		{
			new Player();
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(0.0f, -4.0f, 2.0f);
			var tower = new Tower(TowerType.Water, Vector3D.Zero);
			tower.RenderModel();
		}
	}
}