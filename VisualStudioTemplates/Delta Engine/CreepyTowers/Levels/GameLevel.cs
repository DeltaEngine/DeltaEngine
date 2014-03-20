using System;
using System.Collections.Generic;
using System.IO;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using CreepyTowers.Triggers;
using DeltaEngine.Commands;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.GameLogic.PathFinding;
using DeltaEngine.Rendering3D;
using DeltaEngine.Scenes;

namespace $safeprojectname$.Levels
{
	/// <summary>
	/// Renders the 3D level and manages creeps and pathfinding
	/// </summary>
	public class GameLevel : Level
	{
		//ncrunch: no coverage start
		protected GameLevel(string contentName)
			: base(contentName)
		{
			CreepWaves = new List<CreepWave>();
		}

		public List<CreepWave> CreepWaves { get; private set; }

		//ncrunch: no coverage end

		private void InitializeCamera()
		{
			Camera = new GameCamera(1 / 25.0f, 1 / 10.0f, 0.01f);
			Camera.AllowedMovementRect = Rectangle.FromCenter(Vector2D.Half,
				new Size(Size.Width, Size.Height) * 1.5f);
			Camera.ResetToMinZoom();
		}

		protected internal GameCamera Camera { get; private set; }

		private void AddCreepWaves()
		{
			foreach (var wave in Waves)
				CreepWaves.Add(new CreepWave(wave));
			WaveGenerator = new WaveGenerator(CreepWaves);
		}

		public int DeadCreepCount { get; set; }
		public int ExitReachedCreepCount { get; internal set; }

		protected override void DisposeData()
		{
			DisposeModelAndBackground();
			MoneyUpdated = null;
			LifeUpdated = null;
			this.RemoveModel();
			this.CleanLevel();
			pathfinding = null;
			base.DisposeData();
		}

		private void DisposeModelAndBackground()
		{
			if (levelModel != null)
				levelModel.IsActive = false; //ncrunch: no coverage
			if (scene != null)
				scene.Dispose(); //ncrunch: no coverage
		}

		public event Action LifeUpdated;
		public event Action MoneyUpdated;

		public void Restart()
		{
			this.CleanLevel();
			InitializeGameLevelRoom();
			AddCreepWaves();
		}

		private void InitializeGameLevelRoom()
		{
			pathfinding = new LevelGraph((int)Size.Width, (int)Size.Height);
			for (int count = 0; count < pathfinding.Nodes.Length; count++)
				pathfinding.Nodes[count].Position = GetWorldCoordinates(pathfinding.Nodes[count].Position);
			for (int count = 0; count < SpawnPoints.Count; count++)
				SpawnPoints[count] = GetWorldCoordinates(SpawnPoints[count]);
			for (int count = 0; count < GoalPoints.Count; count++)
				GoalPoints[count] = GetWorldCoordinates(GoalPoints[count]);
			InitializeGameGraph();
		}

		public WaveGenerator WaveGenerator { get; internal set; }
		protected LevelGraph pathfinding;

		public void InitializeGameGraph()
		{
			for (int i = 0; i < MapData.Length; i++)
				if (MapData[i] == LevelTileType.Blocked)
					pathfinding.SetUnreachableAndUpdate(i);
			if (storedPaths.Count == 0)
				for (int i = 0; i < SpawnPoints.Count; i++)
					for (int j = 0; j < GoalPoints.Count; j++)
						storedPaths.Add(GetPath(SpawnPoints[i], GoalPoints[j]));
			UpdateTracers();
		}

		private readonly List<ReturnedPath> storedPaths = new List<ReturnedPath>();

		public ReturnedPath GetPath(Vector2D startNode, Vector2D endNode)
		{
			var indexStart = pathfinding.GetClosestNode(startNode);
			var indexEnd = pathfinding.GetClosestNode(endNode);
			var aStar = new AStarSearch();
			var path = new ReturnedPath();
			if (aStar.Search(pathfinding, indexStart, indexEnd))
				path = aStar.GetPath();
			return path;
		}

		internal void UpdateTracers()
		{
			RemoveCurrentTracers();
			foreach (var path in storedPaths)
				pathMarkers.Add(new PathMarker(path));
		}

		private readonly List<PathMarker> pathMarkers = new List<PathMarker>();

		internal void RemoveCurrentTracers()
		{
			foreach (var pathMarker in pathMarkers)
				pathMarker.IsActive = false;
			pathMarkers.Clear();
		}

		//ncrunch: no coverage start
		public virtual void RenderLevel()
		{
			if (Camera == null)
				return;
			scene = new Scene();
			scene.SetQuadraticBackground("LevelBackgroundSpace");
			levelModel = new Model(ModelName, Vector3D.Zero);
		}

		private Scene scene;
		private Model levelModel;

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			InitializeCamera();
			InitializeGameLevelRoom();
			AddCreepWaves();
			UpdateLevelData();
		}

		protected override void StoreGameTriggers()
		{
			var triggers = data.GetChild("Triggers");
			if (triggers != null)
				foreach (var trigger in triggers.Children)
					InitializeGameTriggers(trigger);
		}

		private void InitializeGameTriggers(XmlData trigger)
		{
			var triggerType = BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound(trigger.Name);
			Triggers.Add(
				Trigger.GenerateTriggerFromType(triggerType, trigger.Name, trigger.Value) as GameTrigger);
		}

		public virtual void SpawnCreep(CreepType type)
		{
			this.SpawnCreepInGame(type);
		}

		public List<ReturnedPath> GetStoredPaths()
		{
			return storedPaths;
		}

		public void RestartTestDisplay()
		{
			pathfinding.ResetGraph();
			storedPaths.Clear();
			pathMarkers.Clear();
			GameLevelExtensions.ResetPlayerGoldAndLives();
			if (MoneyUpdated != null)
				MoneyUpdated();
			UpdateWave();
			UpdateLife();
			foreach (var tower in EntitiesRunner.Current.GetEntitiesOfType<Tower>())
				tower.Dispose();
		}

		public int TotalNumberOfWaves
		{
			get { return CreepWaves.Count; }
		}

		public void UpdateWave()
		{
			if (WaveUpdated != null)
				WaveUpdated();
		}

		public event Action WaveUpdated;

		public void UpdateLife()
		{
			if (LifeUpdated != null)
				LifeUpdated();
		}

		public virtual void SpawnTower(TowerType type, Vector2D position, float rotation = 0.0f)
		{
			this.SpawnTowerInGame(type, position, rotation);
		}

		public void EarnGold(int goldReward)
		{
			Player.Current.Gold += goldReward;
			if (MoneyUpdated != null)
				MoneyUpdated();
		}

		public LevelGraph GetPathFinding()
		{
			return pathfinding;
		}

		protected override void UpdateLevelData()
		{
			this.UpdatePathsIfPossible();
			UpdateTracers();
			base.UpdateLevelData();
		}

		public bool IsCompleted { get; set; }

		protected internal readonly List<Billboard> buildSpotMarkers = new List<Billboard>();

		public Vector2D GetRealPosition(Vector2D point)
		{
			//return GameLevelExtensions.GetGridPosition(point) + Vector2D.Half;
			return GetWorldCoordinates(GameLevelExtensions.GetGridPosition(point));
		}

		internal void FinishChapter()
		{
			ChapterOver.ChapterCompleted();
			if (ChapterFinished != null)
				ChapterFinished();
		}

		public event Action ChapterFinished;
	}
}