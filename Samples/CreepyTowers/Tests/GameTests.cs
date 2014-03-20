using DeltaEngine.Entities;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class GameTests : CreepyTowersGameForTests
	{
		[Test, Ignore]
		public void MessageInsufficientMoney()
		{
			int amountRequired = 0;
			game.InsufficientCredits += i => { amountRequired = i; };
			game.MessageInsufficientMoney(100);
			Assert.AreEqual(100, amountRequired);
		}

		[Test]
		public void MessageCreditsUpdated()
		{
			int difference = 0;
			game.CreditsUpdated += i => { difference = i; };
			game.MessageCreditsUpdated(100);
			Assert.AreEqual(100, difference);
		}

		[Test]
		public void ExitingGameRemovesAllEntities()
		{
			MockGame.ExitGame();
			Assert.AreEqual(0, EntitiesRunner.Current.GetAllEntities().Count);
		}
	}
}