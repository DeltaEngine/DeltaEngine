using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Farseer.Tests
{
	public class PhysicsTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			physics = new FarseerPhysics();
		}

		private FarseerPhysics physics;

		[Test]
		public void CheckDefaultValues()
		{
			Assert.AreEqual(new Vector2D(0.0f, 98.2f), physics.Gravity);
			Assert.AreEqual(0, physics.Bodies.Count());
		}

		[Test]
		public void ChangeGravity()
		{
			physics.Gravity = Vector2D.UnitY;
			Assert.AreEqual(Vector2D.UnitY, physics.Gravity);
		}

		[Test]
		public void CreateBody()
		{
			VerifyBodyIsCreated(physics.CreateRectangle(Size.One));
		}

		private void VerifyBodyIsCreated(PhysicsBody body)
		{
			Assert.IsNotNull(body);
			Assert.AreEqual(1, physics.Bodies.Count());
		}

		[Test]
		public void CreateEdge()
		{
			VerifyBodyIsCreated(physics.CreateEdge(Vector2D.Zero, Vector2D.One));
		}

		[Test]
		public void CreateEdgeMultiPoints()
		{
			VerifyBodyIsCreated(physics.CreateEdge(Vector2Ds));
		}

		private static readonly Vector2D[] Vector2Ds = new[]
		{ Vector2D.Zero, Vector2D.UnitX, Vector2D.One, Vector2D.UnitY };

		[Test]
		public void CreatePolygon()
		{
			VerifyBodyIsCreated(physics.CreatePolygon(Vector2Ds));
		}

		[Test]
		public void CheckWorldIsSimulated()
		{
			var body = physics.CreateRectangle(Size.One);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(Vector2D.Zero, body.Position);
		}
	}
}