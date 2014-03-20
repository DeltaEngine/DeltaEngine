using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace SideScroller.Tests
{
	public class InteractionLogicsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			game = new Game(Resolve<Window>());
			game.StartGame();
		}

		[Test]
		public void FireAShotAtSomeEnemy()
		{
			game.interact.FireShotByPlayer(new Vector2D(0, 0));
		}

		private Game game;

		//ncrunch: no coverage start
		[Test, Ignore]
		public void HeroBulletDisappearsOverTime()
		{
			game.interact.FireShotByPlayer(new Vector2D(0, 0));
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Line2D>().Count);
			AdvanceTimeAndUpdateEntities(DurationOfBullet + 0.01f);
			Assert.AreEqual(0, EntitiesRunner.Current.GetEntitiesOfType<Line2D>().Count);
		}

		private const float DurationOfBullet = 0.2f;
	}
}
