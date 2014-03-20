using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class CreepyTowersGameForTests : TestWithCreepyTowersMockContentLoaderOrVisually
	{
		[SetUp]
		public virtual void Initialize()
		{
			game = new MockGame();
		}

		protected MockGame game;

		[TearDown]
		public void Dispose()
		{
			game.Dispose();
		}
	}
}