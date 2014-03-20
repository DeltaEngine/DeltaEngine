using CreepyTowers.Levels;
using CreepyTowers.Triggers;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Triggers
{
	public class StartingGoldTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		[Test, CloseAfterFirstFrame]
		public void TestStartingGold()
		{
			var trigger = new StartingGold("50");
			Assert.AreEqual(50, trigger.Gold);
			var player = new Player();
			GameTrigger.OnGameStarting();
			Assert.AreEqual(trigger.Gold, player.Gold);
		}
	}
}