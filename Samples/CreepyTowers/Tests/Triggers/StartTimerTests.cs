using CreepyTowers.Levels;
using CreepyTowers.Triggers;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Triggers
{
	public class StartTimerTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		[Test, CloseAfterFirstFrame]
		public void TestStartTimer()
		{
			var trigger = new StartTimer("90");
			Assert.AreEqual(90, trigger.StartingTime);
			var player = new Player();
			GameTrigger.OnGameStarting();
			Assert.AreEqual(trigger.StartingTime, player.Time);
			GameTrigger.OnUpdateEverySecond();
		}
	}
}