using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace TinyPlatformer.Tests
{
	[Ignore]
	public class ActorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			map = new Map(MockMap.JsonNode);
			player = map.actorList.Find(actor => actor.type == "player");
			monster = map.actorList.Find(actor => actor.type == "monster");
			treasure = map.actorList.Find(actor => actor.type == "treasure");
		}

		private Map map;
		private Actor player;
		private Actor monster;
		private Actor treasure;

		[Test, CloseAfterFirstFrame]
		public void CheckMaxVelocityX()
		{
			Assert.AreEqual(10 * Map.Meter, monster.MaxVelocityX);
			Assert.AreEqual(15 * Map.Meter, player.MaxVelocityX);
		}

		[Test]
		public void CheckMaxAcceleration()
		{
			Assert.AreEqual(player.MaxVelocityX / Actor.AccelerationFactor, player.maxAcceleration);
		}

		[Test]
		public void CheckMaxFriction()
		{
			Assert.AreEqual(player.MaxVelocityX / Actor.FrictionFactor, player.maxFriction);
		}

		[Test]
		public void CheckColors()
		{
			Assert.AreEqual(Color.Green, Actor.GetColor("player"));
			Assert.AreEqual(Color.Red, Actor.GetColor("monster"));
			Assert.AreEqual(Color.Yellow, Actor.GetColor("treasure"));
			Assert.AreEqual(Color.Pink, Actor.GetColor("unknown"));
		}

		[Test]
		public void CheckInitialPlayerDesiredMovement()
		{
			Assert.IsFalse(player.WantsToGoLeft);
			Assert.IsFalse(player.WantsToGoRight);
			Assert.IsFalse(player.WantsToJump);
		}

		[Test]
		public void CheckInitialMonsterDesiredMovement()
		{
			Assert.IsFalse(monster.WantsToGoLeft);
			Assert.IsTrue(monster.WantsToGoRight);
			Assert.IsFalse(monster.WantsToJump);
		}

		[Test]
		public void CheckInitialTreasureDesiredMovement()
		{
			Assert.IsFalse(treasure.WantsToGoLeft);
			Assert.IsFalse(treasure.WantsToGoRight);
			Assert.IsFalse(treasure.WantsToJump);
		}
	}
}