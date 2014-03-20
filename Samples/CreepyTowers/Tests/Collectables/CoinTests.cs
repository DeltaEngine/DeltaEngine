using CreepyTowers.Collectables;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Collectables
{
	public class CoinTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void SetUp()
		{
			coin = new Coin(new Vector3D(1, 2, 3), 4);
			coin.RenderModel();
		}

		private Coin coin;

		[Test]
		public void InitialValues()
		{
			Assert.AreEqual(new Vector3D(1, 2, 3), coin.Position);
		}

		[Test]
		public void CollectCoin()
		{
			var player = new Player();
			coin.Collect();
			Assert.AreEqual(4, player.Gold);
		}
	}
}