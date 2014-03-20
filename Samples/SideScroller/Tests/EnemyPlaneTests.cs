using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace SideScroller.Tests
{
	public class EnemyPlaneTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateEnemyPlane()
		{
			enemy = new EnemyPlane(new Vector2D(1.2f, 0.5f));
		}

		private const string EnemyTextureName = "EnemyPlane";
		private EnemyPlane enemy;

		[Test]
		public void LowerLifeWhenHitByBullet()
		{
			enemy = new EnemyPlane(new Vector2D(1.2f, 0.5f));
			Assert.AreEqual(5, enemy.Hitpoints);
			enemy.CheckIfHitAndReact(new Vector2D(1.2f, 0.5f));
			Assert.AreEqual(4, enemy.Hitpoints);
		}

		[Test]
		public void DefeatEnemyPlane()
		{
			enemy = new EnemyPlane(new Vector2D(1.2f, 0.9f));
			bool defeated = false;
			enemy.Destroyed += () => { defeated = true; };
			enemy.ReceiveAttack(5);
			Assert.LessOrEqual(enemy.Hitpoints, 0);
			Assert.IsTrue(defeated);
		}

		[Test]
		public void EnemyDespawnsOutsideScreenArea()
		{
			enemy = new EnemyPlane(new Vector2D(ScreenSpace.Current.Left - enemy.DrawArea.Width, 0.5f));
			AdvanceTimeAndUpdateEntities();
			Assert.IsFalse(enemy.IsActive);
		}
	}
}