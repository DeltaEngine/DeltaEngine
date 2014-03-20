using CreepyTowers.Enemy.Creeps;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CreepWaveTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			creepWave = new CreepWave(5.0f, 1.0f, "Cloth, Iron, Paper, Wood, Glass, Sand, Plastic", "CreepWave");
		}

		private CreepWave creepWave;

		[Test, CloseAfterFirstFrame]
		public void CreepWaveDataCreation()
		{
			Assert.AreEqual(5.0f, creepWave.WaitTime);
			Assert.AreEqual(1.0f, creepWave.SpawnInterval);
			Assert.AreEqual(7, creepWave.CreepsAndGroupsList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CreepWaveDataCreationFromWave()
		{
			var wave = new Wave(5.0f, 1.0f, "Cloth, Iron, Paper, Wood, Glass, Sand, Plastic", "CreepWave");
			creepWave = new CreepWave(wave);
			Assert.AreEqual(5.0f, creepWave.WaitTime);
			Assert.AreEqual(1.0f, creepWave.SpawnInterval);
			Assert.AreEqual(7, creepWave.CreepsAndGroupsList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckCreepList()
		{
			Assert.AreEqual(7, creepWave.CreepsAndGroupsList.Count);
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Cloth));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Glass));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Iron));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Paper));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Plastic));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Sand));
			Assert.IsTrue(creepWave.CreepsAndGroupsList.Contains(CreepType.Wood));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForGroupSquad()
		{
			creepWave = new CreepWave(5.0f, 1.0f, "Paper3", "Paper3");
			if (creepWave.CreepsAndGroupsList[0].GetType() == typeof(GroupData))
			{
				var group = (GroupData)creepWave.CreepsAndGroupsList[0];
				Assert.AreEqual(3, group.CreepList.Count);
				Assert.AreEqual(CreepType.Paper, group.CreepList[0]);
				Assert.AreEqual(CreepType.Paper, group.CreepList[1]);
				Assert.AreEqual(CreepType.Paper, group.CreepList[2]);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForGroupGarbage()
		{
			creepWave = new CreepWave(5.0f, 1.0f, "Plastic2", "Plastic3");
			if (creepWave.CreepsAndGroupsList[0].GetType() == typeof(GroupData))
			{
				var group = (GroupData)creepWave.CreepsAndGroupsList[0];
				Assert.AreEqual(2, group.CreepList.Count);
				Assert.AreEqual(CreepType.Plastic, group.CreepList[0]);
				Assert.AreEqual(CreepType.Plastic, group.CreepList[1]);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForGroupMix()
		{
			creepWave = new CreepWave(5.0f, 1.0f, "GroupPpCl", "PaperCloth");
			if (creepWave.CreepsAndGroupsList[0].GetType() == typeof(GroupData))
			{
				var group = (GroupData)creepWave.CreepsAndGroupsList[0];
				Assert.AreEqual(2, group.CreepList.Count);
				Assert.AreEqual(CreepType.Paper, group.CreepList[0]);
				Assert.AreEqual(CreepType.Cloth, group.CreepList[1]);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForGroupPpPl()
		{
			creepWave = new CreepWave(5.0f, 1.0f, "GroupPpPl", "GroupPpPl");
			if (creepWave.CreepsAndGroupsList[0].GetType() == typeof(GroupData))
			{
				var group = (GroupData)creepWave.CreepsAndGroupsList[0];
				Assert.AreEqual(2, group.CreepList.Count);
				Assert.AreEqual(CreepType.Paper, group.CreepList[0]);
				Assert.AreEqual(CreepType.Plastic, group.CreepList[1]);
			}
		}

		[Test, CloseAfterFirstFrame]
		public void CheckTotalCreepCountForAWave()
		{
			var waveA = new CreepWave(5.0f, 1.0f, "Cloth, Iron, Paper, Wood, Glass, Sand, Plastic", 
				"MixWave1");
			Assert.AreEqual(7, waveA.TotalCreepsInWave);
			var waveB = new CreepWave(5.0f, 1.0f, "Cloth, Iron, Paper, Paper2", "MixWave2");
			Assert.AreEqual(5, waveB.TotalCreepsInWave);
			var waveC = new CreepWave(5.0f, 1.0f,
				"GroupPpPl, GroupPpCl, GroupClPl, Plastic3, Cloth3, Paper3, Cloth2", "MixWave3");
			Assert.AreEqual(17, waveC.TotalCreepsInWave);
		}

		[Test, CloseAfterFirstFrame]
		public void AsXmlData()
		{
			var xml = creepWave.AsXmlData();
			Assert.AreEqual("5", xml.GetAttributeValue("WaitTime"));
			Assert.AreEqual("1", xml.GetAttributeValue("SpawnInterval"));
		}

		[Test, CloseAfterFirstFrame]
		public void WaveToString()
		{
			Assert.AreEqual("CreepWave 5, 1, 0, 1, Cloth, Iron, Paper, Wood, Glass, Sand, Plastic",
				creepWave.ToString());
		}
	}
}