using System.Collections.Generic;
using CreepyTowers.Levels;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;

namespace $safeprojectname$.Enemy.Creeps
{
	public class WaveGenerator : Entity
	{
		public WaveGenerator(List<CreepWave> waveList, Vector3D spawnPoint = default(Vector3D))
		{
			level = (GameLevel)Level.Current;
			this.waveList = waveList;
			this.spawnPoint = spawnPoint;
			UpdateTotalCreepCountForLevel();
			level.UpdateWave();
			Start<WaveCreation>();
		}

		internal readonly List<CreepWave> waveList;
		public readonly Vector3D spawnPoint;
		public readonly GameLevel level;
		public int TotalCreepsInLevel { get; set; }

		public void UpdateTotalCreepCountForLevel()
		{
			TotalCreepsInLevel = 0;
			if (waveList != null || waveList.Count > 0)
				foreach (CreepWave wave in waveList)
					TotalCreepsInLevel += wave.TotalCreepsInWave;
		}

		public override void Dispose()
		{
			base.Dispose();
			waveList.Clear();
			WaveCreation.creepCountForCurrentWave = 0;
			Stop<WaveCreation>();
		}

		//ncrunch: no coverage start
		public class WaveCreation : UpdateBehavior
		{
			public WaveCreation()
				: base(Priority.Normal, true)
			{
				creepCountForCurrentWave = 0;
				//TODO: needs to be changed!
				//var waveManager = EntitiesRunner.Current.GetEntitiesOfType<WaveGenerator>()[0];
				//if (waveManager.ContainsBehavior<SpawnAllCreepsInGroup>())
				//	waveManager.Stop<SpawnAllCreepsInGroup>();
			}

			internal static int creepCountForCurrentWave;

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (WaveGenerator generator in entities)
				{
					if (generator.waveList == null || generator.waveList.Count == 0)
						continue;
					var wave = generator.waveList[0];
					if (Time.CheckEvery(wave.WaitTime))
						CreateWave(wave, generator);
				}
			}

			private static void CreateWave(CreepWave wave, WaveGenerator waveGenerator)
			{
				if (creepCountForCurrentWave >= wave.TotalCreepsInWave)
				{
					creepCountForCurrentWave = 0;
					waveGenerator.waveList.RemoveAt(0);
					waveGenerator.level.UpdateWave();
					return;
				}
				if (!Time.CheckEvery(wave.SpawnInterval))
					return;
				SpawnNextItemFromList(wave.CreepsAndGroupsList[0], waveGenerator);
			}

			private static void SpawnNextItemFromList(object itemInWaveList, WaveGenerator waveGenerator)
			{
				if (IsItemCreep(itemInWaveList))
				{
					if (waveGenerator.spawnPoint == default(Vector3D))
						waveGenerator.level.SpawnCreep((CreepType)itemInWaveList);
					else
						new Creep((CreepType)itemInWaveList, waveGenerator.spawnPoint);
					creepCountForCurrentWave++;
					waveGenerator.waveList[0].CreepsAndGroupsList.RemoveAt(0);
				}
				else if (IsItemGroup(itemInWaveList))
				{
					if (!waveGenerator.Contains<SpawnAllCreepsInGroup>())
						waveGenerator.Start<SpawnAllCreepsInGroup>();
					waveGenerator.Stop<WaveCreation>();
				}
			}

			private static bool IsItemGroup(object itemInWaveList)
			{
				return itemInWaveList.GetType() == typeof(GroupData);
			}

			private static bool IsItemCreep(object itemInWaveList)
			{
				return itemInWaveList.GetType() == typeof(CreepType);
			}

			public class SpawnAllCreepsInGroup : UpdateBehavior
			{
				public SpawnAllCreepsInGroup()
					: base(Priority.Normal, true)
				{
					creepsSpawnedFromCurrentGroup = 0;
					//TODO: needs to be changed!
					//var waveManager = EntitiesRunner.Current.GetEntitiesOfType<WaveGenerator>()[0];
					//var wave = waveManager.waveList[0];
					creepCount = /*((GroupData)wave.CreepsAndGroupsList[0]).CreepList.Count*/ 0;
				}

				private int creepsSpawnedFromCurrentGroup;
				private readonly int creepCount;

				public override void Update(IEnumerable<Entity> entities)
				{
					foreach (WaveGenerator waveManager in entities)
					{
						if (waveManager.waveList.Count == 0)
							return;
						SpawnCreepsFromGroup(waveManager);
					}
				}

				private void SpawnCreepsFromGroup(WaveGenerator waveGenerator)
				{
					var wave = waveGenerator.waveList[0];
					if (creepsSpawnedFromCurrentGroup >= creepCount)
					{
						UpdateCreepCountAndStopCurrentBehavior(waveGenerator);
						return;
					}
					var groupData = (GroupData)wave.CreepsAndGroupsList[0];
					if (!Time.CheckEvery(groupData.CreepSpawnInterval))
						return;
					waveGenerator.level.SpawnCreep(groupData.CreepList[0]);
					groupData.CreepList.RemoveAt(0);
					creepsSpawnedFromCurrentGroup++;
				}

				private void UpdateCreepCountAndStopCurrentBehavior(WaveGenerator waveGenerator)
				{
					creepCountForCurrentWave += creepCount;
					creepsSpawnedFromCurrentGroup = 0;
					waveGenerator.Start<WaveCreation>();
					waveGenerator.waveList[0].CreepsAndGroupsList.RemoveAt(0);
					waveGenerator.Stop<SpawnAllCreepsInGroup>();
				}
			}
		}
	}
}