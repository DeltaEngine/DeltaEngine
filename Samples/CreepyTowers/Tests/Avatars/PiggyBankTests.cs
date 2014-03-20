using System.Collections.Generic;
using CreepyTowers.Avatars;
using CreepyTowers.Collectables;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace CreepyTowers.Tests.Avatars
{
	public class PiggyBankTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void SetUp()
		{
			new Level(new Size(5, 5));
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.2f, 0.3f }));
			pig = new PiggyBank();
			pig.PerformAttack(AvatarAttack.PiggyBankCoinMinefield, Vector2D.Unused);
			coins = EntitiesRunner.Current.GetEntitiesOfType<Coin>();
		}

		private PiggyBank pig;
		private List<Coin> coins;
		
		[Test]
		public void SpawnCoinMinefield()
		{
			Assert.AreEqual(5, coins.Count);
			Assert.AreEqual(new Vector3D(0.5f, 1.5f, 0.0f), coins[0].Position);
			Assert.IsTrue(coins[0].IsActive);
		}

		[Test]
		public void TriggeringPiggyBankPayDayMakesCreepsDropMoreGold()
		{
			var creep = new Creep(CreepType.Cloth, new Vector2D(0.5f, 1.5f));
			var gold = creep.GetStatValue("Gold");
			pig.PerformAttack(AvatarAttack.PiggyBankPayDay, Vector2D.Unused);
			AdvanceTimeAndUpdateEntities(4.9f);
			Assert.AreEqual(1.5f, creep.GetStatValue("Gold") / gold);
		}

		[Test]
		public void PiggyBankPayDayBuffLasts5Seconds()
		{
			var creep = new Creep(CreepType.Cloth, new Vector2D(0.5f, 1.5f));
			var gold = creep.GetStatValue("Gold");
			pig.PerformAttack(AvatarAttack.PiggyBankPayDay, Vector2D.Unused);
			AdvanceTimeAndUpdateEntities(5.1f);
			Assert.GreaterOrEqual(creep.GetStatValue("Gold"), gold);
		}
	}
}