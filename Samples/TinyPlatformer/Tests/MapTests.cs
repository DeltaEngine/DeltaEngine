using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	[Ignore]
	public class MapTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			map = new Map(MockMap.JsonNode);
		}

		private Map map;

		[Test, CloseAfterFirstFrame]
		public void CheckMapSize()
		{
			Assert.AreEqual(8, map.width);
			Assert.AreEqual(6, map.height);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckMapContent()
		{
			Assert.AreEqual(BlockType.LevelBorder, map.Blocks[0, 0]);
			Assert.AreEqual(BlockType.None, map.Blocks[1, 1]);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckEntityLoading()
		{
			Assert.AreEqual(1, map.actorList.FindAll(actor => actor.type == "player").Count);
			Assert.AreEqual(1, map.actorList.FindAll(actor => actor.type == "monster").Count);
			Assert.AreEqual(1, map.actorList.FindAll(actor => actor.type == "treasure").Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckGetColor()
		{
			Assert.AreEqual(Color.Gold, Map.GetColor(BlockType.Gold));
			Assert.AreEqual(Color.Orange, Map.GetColor(BlockType.GroundBrick));
			Assert.AreEqual(Color.Red, Map.GetColor(BlockType.PlatformBrick));
			Assert.AreEqual(Color.Purple, Map.GetColor(BlockType.PlatformTop));
			Assert.AreEqual(Color.Teal, Map.GetColor(BlockType.LevelBorder));
			Assert.AreEqual(Color.TransparentBlack, Map.GetColor(BlockType.None));
		}

		[Test, CloseAfterFirstFrame]
		public void ScoreStartsAtZero()
		{
			Assert.AreEqual(0, map.score);
			Assert.AreEqual("Score: 0", map.scoreText.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void TreasureAddsOneToScore()
		{
			map.AddToScore("treasure");
			Assert.AreEqual(1, map.score);
			Assert.AreEqual("Score: 1", map.scoreText.Text);
		}

		[Test, CloseAfterFirstFrame]
		public void MonsterKillAddsThreeToScore()
		{
			map.AddToScore("monster");
			Assert.AreEqual(3, map.score);
			Assert.AreEqual("Score: 3", map.scoreText.Text);
		}
	}
}