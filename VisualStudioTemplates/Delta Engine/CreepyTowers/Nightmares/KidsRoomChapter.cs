using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.GUI;
using CreepyTowers.Levels;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D;
using Nightmare1 = CreepyTowers.GUI.Nightmare1;
using TowerSelectionPanel = CreepyTowers.GUI.TowerSelectionPanel;

namespace $safeprojectname$.Nightmares
{
	public class KidsRoomChapter : Chapter
	{
		public KidsRoomChapter()
			: base(LevelMaps.N1C1ChildsRoomLevelInfo)
		{
			SetupChapter();
			AddEventActions();
			Current = this;
		}

		private void SetupChapter()
		{
			ShowTutorial();
			StopWaveGenerator();
			SetInactiveTowerPanelButtons();
		}

		private static void SetInactiveTowerPanelButtons()
		{
			var towerPanel =
				(TowerSelectionPanel)MenuController.Current.GetMenu(GameMenus.TowerSelectionPanel);
			towerPanel.InactiveButtonsList = new List<string>
			{
				TowerType.Acid.ToString(),
				TowerType.Ice.ToString(),
				TowerType.Impact.ToString(),
				TowerType.Slice.ToString(),
				TowerType.Water.ToString(),
			};
		}

		private void ShowTutorial()
		{
			tutorialScene = (TutorialCinematics)MenuController.Current.GetMenu(GameMenus.SceneTutorial);
			MenuController.Current.ShowMenu(GameMenus.SceneTutorial);
			Time.IsPaused = true;
		}

		private TutorialCinematics tutorialScene;

		private void AddEventActions()
		{
			towerCount = 0;
			Tower.WasBuilt += ActionIfOneTowerWasBuilt;
			Creep.WasSpawned += ActionIfCreepIsSpawned;
		}

		private void ActionIfOneTowerWasBuilt()
		{
			towerCount = EntitiesRunner.Current.GetEntitiesOfType<Tower>().Count;
			if (towerCount != 1)
				return;
			UpdateTutorialScene();
		}

		private void UpdateTutorialScene()
		{
			MenuController.Current.ShowMenu(GameMenus.SceneTutorial);
			tutorialScene.UpdateAvatarText();
			Time.IsPaused = true;
		}

		private int towerCount;

		private void ActionIfCreepIsSpawned(Creep creep)
		{
			creep.IsDead += () =>
			{
				if (GameLevel.DeadCreepCount == 1)
				{
					UpdateTutorialScene();
					GameLevel.WaveGenerator.Stop<WaveGenerator.WaveCreation>();
				}
			};
		}

		protected override void UpdateCamera()
		{
			GameLevel.Camera.ResetPositionToDefault();
			GameLevel.Camera.MinZoom = 1 / 30.0f;
			GameLevel.Camera.MaxZoom = 1 / 10.0f;
		}

		protected override void InitializePlayer()
		{
			var player = Player.Current;
			player.Gold = 4000;
			player.MaxLives = 5;
			player.Gems = 10;
		}

		protected override void InitializeLevel()
		{
			base.InitializeLevel();
			PlaceTowerMarkers();
		}

		private void PlaceTowerMarkers()
		{
			var placeableTiles = GameLevel.GetAllTilesOfType(LevelTileType.Placeable);
			foreach (Vector2D tile in placeableTiles)
				GameLevel.buildSpotMarkers.Add(new Billboard(new Vector3D(tile, 0.01f), Size.One,
					ContentLoader.Load<Material>("TowerBuildingSpotMat"), BillboardMode.Ground));
		}

		private void StopWaveGenerator()
		{
			GameLevel.WaveGenerator.Stop<WaveGenerator.WaveCreation>();
		}

		protected override void Completed()
		{
			Nightmare1.ChapterToUnlock = 2;
		}

		public override void Dispose()
		{
			RemoveBuildSpotMarkers();
			Creep.WasSpawned -= ActionIfCreepIsSpawned;
			Tower.WasBuilt -= ActionIfOneTowerWasBuilt;
			base.Dispose();
		}

		private void RemoveBuildSpotMarkers()
		{
			foreach (var billboard in GameLevel.buildSpotMarkers)
				billboard.IsActive = false;
			GameLevel.buildSpotMarkers.Clear();
		}

		public override void Restart()
		{
			base.Restart();
			tutorialScene.MessageCount = 0;
			SetupChapter();
		}
	}
}