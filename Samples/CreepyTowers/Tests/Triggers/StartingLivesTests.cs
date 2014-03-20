using CreepyTowers.Levels;
using CreepyTowers.Triggers;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Triggers
{
	public class StartingLivesTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		[Test, CloseAfterFirstFrame]
		public void TestStartingLives()
		{
			var trigger = new StartingLives("5");
			Assert.AreEqual(5, trigger.Lives);
			var player = new Player();
			GameTrigger.OnGameStarting();
			Assert.AreEqual(trigger.Lives, player.LivesLeft);
		}
	}
}