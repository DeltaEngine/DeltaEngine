using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class WaveGeneratorTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			var waveA = new CreepWave(1.0f, 1.0f, "Paper, Paper, Paper, Paper, Cloth, Cloth", "CreepWave");
			waveList = new List<CreepWave> { waveA };
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
			new Player();
		}

		private List<CreepWave> waveList;

		[Test, CloseAfterFirstFrame]
		public void SpawnCreepWaves()
		{
			new WaveGenerator(waveList, Vector3D.One);
		}

		[Test, CloseAfterFirstFrame]
		public void CreepIsSpawnedAtTheSpawnPoint()
		{
			var creepPos = new Vector3D(2.0f, 3.0f, 1.0f);
			new WaveGenerator(waveList, creepPos);
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.AreEqual(creepPos, EntitiesRunner.Current.GetEntitiesOfType<Creep>()[0].Position);
		}

		[Test, CloseAfterFirstFrame]
		public void IfNoWaveDataThenNoWavesSpawned()
		{
			var waves = new List<CreepWave>();
			new WaveGenerator(waves, Vector3D.One);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void WaveCreateOnlyAfterWaitTime()
		{
			new WaveGenerator(waveList, Vector3D.One);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CreepSpawnedInWaveOnlyAfterSpawnInterval()
		{
			new WaveGenerator(waveList, Vector3D.One);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.GreaterOrEqual(EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count, 1);
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.GreaterOrEqual(EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count, 2);
		}

		[Test, CloseAfterFirstFrame]
		public void NoCreepsSpawnedIfCreepListIsEmpty()
		{
			var waveA = new CreepWave(0.0f, 1.0f);
			var waves = new List<CreepWave> { waveA };
			new WaveGenerator(waves, Vector3D.One);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void FirstItemInWaveListHasWaveWaitTimeAddedToItsSpawnInterval()
		{
			var waveA = new CreepWave(0.05f, 0.05f, "Paper, Squad, Paper", "CreepWave");
			waveList = new List<CreepWave> { waveA };
			new WaveGenerator(waveList, Vector3D.One);
			AdvanceTimeAndUpdateEntities(5.0f);
			Assert.AreEqual(2, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void NextWaveIsSpawnedOnlyAfterElapsedTimeIsOver()
		{
			var waveA = new CreepWave(0.1f, 0.1f, "Paper, Paper", "PaperWave");
			var waveB = new CreepWave(0.3f, 0.1f, "Wood", "WoodWave");
			new WaveGenerator(new List<CreepWave> { waveA, waveB }, Vector3D.One);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(2, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(0.4f);
			Assert.AreEqual(3, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void TestTheTotalOfNumberOfCreepsInLevel()
		{
			var waveA = new CreepWave(1.0f, 1.0f, "Paper, Paper", "PaperWave");
			var generator = new WaveGenerator(new List<CreepWave> { waveA }, Vector3D.One);
			AdvanceTimeAndUpdateEntities(1.01f);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(2, generator.TotalCreepsInLevel);
		}

		//ncrunch: no coverage start
		[Test, CloseAfterFirstFrame, Category("Slow")]
		public void NextCreepIsSpawnedOnlyAfterCreepSpawnTimeInterval()
		{
			if (!IsMockResolver)
				return;
			var waveA = new CreepWave(1.0f, 1.0f, "Paper, Paper", "PaperWave");
			new WaveGenerator(new List<CreepWave> { waveA }, Vector3D.One);
			AdvanceTimeAndUpdateEntities(1.01f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
		}
	}
}