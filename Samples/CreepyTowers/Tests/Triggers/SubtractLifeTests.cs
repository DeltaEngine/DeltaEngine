using CreepyTowers.Levels;
using CreepyTowers.Triggers;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Triggers
{
	public class SubtractLifeTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		[Test, CloseAfterFirstFrame]
		public void TestSubtractLife()
		{
			new SubtractLife();
			var player = new Player();
			GameTrigger.OnEnemyReachGoal();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(0, player.LivesLeft);
		}
	}
}