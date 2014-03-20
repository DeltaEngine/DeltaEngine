using System;
using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using CreepyTowers.Triggers;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering3D;

namespace $safeprojectname$.Levels
{
	public static class GameLevelExtensions
	{
		public static void ResetPlayerGoldAndLives()
		{
			var player = Player.Current;
			player.Gold = 4000;
			player.LivesLeft = 5;
		}

		public static Vector2D GetGridPosition(Vector2D point)
		{
			var result = Level.GetIntersectionWithFloor(point);
			if (result == null)
				throw new IntersectionNotFound(point);
			var position = (Vector3D)result;
			return Level.Current.GetMapCoordinates(position.GetVector2D());
		}

		private class IntersectionNotFound : Exception
		{
			public IntersectionNotFound(Vector2D position)
				: base(position.ToString()) {}
		}

		public static void AddCreepWaveToWaveGenerator(this GameLevel gameLevel, CreepWave wave)
		{
			gameLevel.WaveGenerator.waveList.Add(wave);
			gameLevel.WaveGenerator.UpdateTotalCreepCountForLevel();
			gameLevel.UpdateWave();
		}

		public static void SpawnCreepInGame(this GameLevel gameLevel, CreepType type)
		{
			var path = gameLevel.GetPathForCreep().ToList();
			var creep = gameLevel.CreateAndShowCreep(type, path[0], path[path.Count - 1]);
			foreach (var position in path)
				creep.Path.Add(new Vector2D(position.X, position.Y));
		}

		private static IEnumerable<Vector2D> GetPathForCreep(this GameLevel gameLevel)
		{
			var storedPaths = gameLevel.GetStoredPaths();
			if (storedPaths.Count == 1)
				return storedPaths[0].GetListOfCoordinates();
			int max = 0;
			int min = 0;
			for (int i = 1; i < storedPaths.Count; i++)
				if (storedPaths[i].FinalCost < storedPaths[min].FinalCost)
					min = i;
				else if (storedPaths[i].FinalCost > storedPaths[max].FinalCost)
					max = i;
			var valueMin = storedPaths[min].FinalCost / storedPaths[min].GetListOfCoordinates().Count;
			var valueMax = storedPaths[max].FinalCost / storedPaths[max].GetListOfCoordinates().Count;
			return Math.Abs(valueMax - valueMin) <= 50
				? storedPaths[Randomizer.Current.Get(0, storedPaths.Count)].GetListOfCoordinates()
				: storedPaths[min].GetListOfCoordinates();
		}

		private static Creep CreateAndShowCreep(this GameLevel gameLevel, CreepType type,
			Vector2D spawnPoint, Vector2D finalTarget)
		{
			var creep = new Creep(type, spawnPoint) { FinalTarget = finalTarget };
			creep.RenderModel();
			creep.IsDead += () =>
			{
				gameLevel.EarnGold((int)creep.GetStatValue("Gold"));
				gameLevel.DeadCreepCount++;
				gameLevel.CheckChapterCompletion();
			};
			creep.ReachedExit += gameLevel.ReduceOneLife;
			return creep;
		}

		private static void CheckChapterCompletion(this GameLevel gameLevel)
		{
			if (!HasPlayerFailed())
				CheckForPlayerSuccess(gameLevel);
		}

		private static bool HasPlayerFailed()
		{
			if (Player.Current.LivesLeft > 0)
				return false;
			ChapterOver.ChaptedFailed();
			return true;
		}

		private static void CheckForPlayerSuccess(this GameLevel gameLevel)
		{
			if (gameLevel.DeadCreepCount + gameLevel.ExitReachedCreepCount <
				gameLevel.WaveGenerator.TotalCreepsInLevel)
				return;
			gameLevel.FinishChapter();
		}

		private static void ReduceOneLife(this GameLevel gameLevel)
		{
			Player.Current.LivesLeft--;
			gameLevel.ExitReachedCreepCount++;
			if (IsPlayerLifeLessThan20Percent())
				PlaySound(GameSounds.LowHealth);
			gameLevel.UpdateLife();
			gameLevel.CheckChapterCompletion();
		}

		private static bool IsPlayerLifeLessThan20Percent()
		{
			return Player.Current.LivesLeft <= 0.2 * Player.Current.MaxLives;
		}

		private static void PlaySound(GameSounds soundName)
		{
			var sound = ContentLoader.Load<Sound>(soundName.ToString());
			if (sound != null)
				sound.Play();
		}

		public static void SpawnTowerInGame(this GameLevel level, TowerType type,
			Vector2D gridPosition, float rotation = 0.0f)
		{
			var properties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			if (IsCreepInTile(gridPosition) || Player.Current.Gold < properties.Get(type).Cost)
				return;
			var index = level.GetIndexForMapData(gridPosition);
			if (level.MapData[index] != LevelTileType.Placeable)
				return;
			level.SetUnreacheableTile(gridPosition, type);
			if (!level.IsPossibleAddTower(gridPosition))
				return;
			var towerPosInWorldSpace = level.GetWorldCoordinates(gridPosition);
			var tower = new Tower(type, towerPosInWorldSpace, rotation);
			tower.RenderModel();
			level.EarnGold(-properties.Get(type).Cost);
		}

		private static bool IsCreepInTile(Vector2D position)
		{
			foreach (var creep in EntitiesRunner.Current.GetEntitiesOfType<Creep>())
			{
				var creepTile = creep.Position - Vector2D.Half;
				if ((creepTile.X - position.X).Abs() <= 1.0f && (creepTile.Y - position.Y).Abs() <= 1.0f &&
					creep.Path.Contains(position + Vector2D.Half))
					return true;
			}
			return false;
		}

		private static void SetUnreacheableTile(this GameLevel level, Vector2D position,
			TowerType type)
		{
			var pathfinding = level.GetPathFinding();
			var index = (int)(position.X + position.Y * level.Size.Width);
			level.MapData[index] = LevelTileType.Blocked;
			pathfinding.SetUnreachableAndUpdate(index);
			var towerProperties = ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString());
			var buff = new BuffEffect(Player.Current.Avatar.GetType().Name + "RangeMultiplier");
			var range = towerProperties.Get(type).Range;
			range *= buff.Multiplier > 0.0f ? buff.Multiplier : 1.0f;
			pathfinding.UpdateWeightInAdjacentNodes(position, (int)range, 100);
		}

		private static bool IsPossibleAddTower(this GameLevel level, Vector2D gridPosition)
		{
			var index = level.GetIndexForMapData(gridPosition);
			if (level.UpdateExistingCreeps(gridPosition + Vector2D.Half) &&
				level.UpdatePathsIfPossible())
				return true;
			level.MapData[index] = LevelTileType.Placeable;
			level.GetPathFinding().SetReachableAndUpdate(index);
			return false;
		}

		private static bool UpdateExistingCreeps(this GameLevel gameLevel, Vector2D position)
		{
			foreach (var creep in EntitiesRunner.Current.GetEntitiesOfType<Creep>())
			{
				if (creep.Path.Count == 0 || !creep.Path.Contains(position))
					continue;
				if (!IsTherePossibleExitPath(gameLevel, creep))
					return false;
			}
			return true;
		}

		private static bool IsTherePossibleExitPath(this GameLevel gameLevel, Creep creep)
		{
			var isThereWay = false;
			for (int i = 0; i < gameLevel.GoalPoints.Count; i++)
			{
				var list =
					gameLevel.GetPath(creep.Position.GetVector2D(), gameLevel.GoalPoints[i]).
					          GetListOfCoordinates();
				if (list.Count == 0)
					continue;
				creep.Path = list.Select(element => element + Vector2D.Half).ToList();
				creep.FinalTarget = creep.Path[creep.Path.Count - 1];
				isThereWay = true;
				break;
			}
			return isThereWay;
		}

		public static bool UpdatePathsIfPossible(this GameLevel gameLevel)
		{
			for (int i = 0; i < gameLevel.SpawnPoints.Count; i++)
				for (int j = 0; j < gameLevel.GoalPoints.Count; j++)
				{
					var list = gameLevel.GetPath(gameLevel.SpawnPoints[i], gameLevel.GoalPoints[j]);
					if (list.GetListOfCoordinates().Count == 0)
						return false;
					var storedPaths = gameLevel.GetStoredPaths();
					storedPaths[i * gameLevel.SpawnPoints.Count + j] = list;
				}
			gameLevel.UpdateTracers();
			return true;
		}

		public static void CleanLevel(this GameLevel level)
		{
			if (level == null)
				return;
			level.RemoveTowers();
			level.RemoveCreeps();
			level.GetStoredPaths().Clear();
			level.RemoveCurrentTracers();
			ResetPlayerGoldAndLives();
			level.DeadCreepCount = 0;
			level.ExitReachedCreepCount = 0;
			level.WaveGenerator.Dispose();
			level.IsCompleted = false;
		}

		public static void RemoveTowers(this GameLevel level)
		{
			foreach (var tower in EntitiesRunner.Current.GetEntitiesOfType<Tower>())
			{
				var towerMapPos = level.GetMapCoordinates(tower.Position.GetVector2D());
				var index = level.GetIndexForMapData(towerMapPos);
				if(index >= level.MapData.Length)
					return;
				level.MapData[index] = LevelTileType.Placeable;
				level.GetPathFinding().SetReachableAndUpdate(index);
				tower.Dispose();
			}
		}

		public static void RemoveCreeps(this GameLevel level)
		{
			var allCreeps = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			foreach (Creep creep in allCreeps)
				creep.Dispose();
		}

		public static void RemoveModel(this GameLevel level)
		{
			var models = EntitiesRunner.Current.GetEntitiesOfType<Model>();
			foreach (Model model in models)
				model.Dispose();
		}

		public static void SellTower(this GameLevel level, Vector2D position)
		{
			var list = EntitiesRunner.Current.GetEntitiesOfType<Tower>();
			foreach (var tower in list)
			{
				var towerTile = tower.Position - Vector2D.Half;
				if (towerTile != position)
					continue;
				PlaySound(GameSounds.TowerSell);
				level.EarnGold((int)(tower.GetStatValue("Cost") / 2));
				level.GetPathFinding().UpdateWeightInAdjacentNodes(position,
					(int)(tower.GetStatValue("Range")), -100);
				tower.Dispose();
				level.UpdateGridAndExistingCreeps(position);
				return;
			}
		}

		private static void UpdateGridAndExistingCreeps(this GameLevel level, Vector2D gridPosition)
		{
			var index = level.GetIndexForMapData(gridPosition);
			level.MapData[index] = LevelTileType.Placeable;
			level.GetPathFinding().SetReachableAndUpdate(index);
			foreach (var creep in EntitiesRunner.Current.GetEntitiesOfType<Creep>())
				level.IsTherePossibleExitPath(creep);
			level.UpdatePathsIfPossible();
		}
	}
}