using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class BodyTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateBody()
		{
			physics = Resolve<Physics>();
			physics.Gravity = Vector2D.UnitY;
			body = physics.CreateRectangle(new Size(0.5f));
		}

		private Physics physics;
		private PhysicsBody body;

		[TearDown]
		public void DisposeBody()
		{
			body.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeLinearVelocity()
		{
			body.LinearVelocity = new Vector2D(5, -5);
			Assert.AreEqual(new Vector2D(5, -5), body.LinearVelocity);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRotation()
		{
			body.Rotation = 90;
			Assert.AreEqual(90, body.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePosition()
		{
			body.Position = Vector2D.One;
			Assert.AreEqual(Vector2D.One, body.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeStatic()
		{
			body.IsStatic = true;
			Assert.IsTrue(body.IsStatic);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRestitution()
		{
			body.Restitution = 1;
			Assert.AreEqual(1, body.Restitution);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeFriction()
		{
			body.Friction = 0.5f;
			Assert.AreEqual(0.5f, body.Friction);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyLinearImpulse()
		{
			body.ApplyLinearImpulse(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(0.1f);
			Assert.AreNotEqual(Vector2D.Zero, body.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyAngularImpulse()
		{
			body.ApplyAngularImpulse(10.0f);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(0.0f, body.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyTorque()
		{
			body.ApplyTorque(10.0f);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(0.0f, body.Rotation);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateEdgeFromTwoSinglePoints()
		{
			Vector2D[] edges = { Vector2D.Zero, Vector2D.Half };
			var edge = physics.CreateEdge(Vector2D.Zero, Vector2D.Half);
			Assert.AreEqual(edges, edge.LineVertices);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateEdgeFromPointArray()
		{
			Vector2D[] edges = { Vector2D.Zero, Vector2D.Half };
			var edge = physics.CreateEdge(edges);
			Assert.AreEqual(edges, edge.LineVertices);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateCircleAndGetVertices()
		{
			var circle = physics.CreateCircle(0.5f);
			Assert.IsNotEmpty(circle.LineVertices);
		}

		[Test, CloseAfterFirstFrame]
		public void CreatePolygonFromPoints()
		{
			Vector2D[] points = { Vector2D.Zero, Vector2D.Half, Vector2D.One, Vector2D.UnitX };
			var polygon = physics.CreatePolygon(points);
			Assert.IsNotEmpty(polygon.LineVertices);
		}
	}
}