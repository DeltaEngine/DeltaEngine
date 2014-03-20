using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Farseer.Tests
{
	public class BodyTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			physics = new FarseerPhysics();
		}

		private Physics physics;

		[Test]
		public void TestBodyDefaultIsNotStatic()
		{
			var body = physics.CreateCircle(45.0f);
			Assert.IsFalse(body.IsStatic);
		}

		[Test]
		public void TestBodyDefaultSetStatic()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.IsStatic = true;
			Assert.IsTrue(body.IsStatic);
		}

		[Test]
		public void TestBodyDefaultFriction()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.AreEqual(body.Friction, DefaultFriction);
		}

		private const float DefaultFriction = 0.2f;

		[Test]
		public void TestBodySetFriction()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Friction = 0.5f;
			Assert.AreEqual(body.Friction, 0.5f);
		}

		[Test]
		public void TestBodyDefaultPosition()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.AreEqual(body.Position, Vector2D.Zero);
		}

		[Test]
		public void TestBodySetPosition()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Position = new Vector2D(100, 100);
			Assert.AreEqual(body.Position, new Vector2D(100, 100));
		}

		[Test]
		public void TestBodyRestitution()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.Restitution = 0.5f;
			Assert.AreEqual(body.Restitution, 0.5f);
		}

		[Test]
		public void TestStaticBodyNoRotation()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			body.IsStatic = true;
			Assert.AreEqual(body.Rotation, 0.0f);
		}

		[Test]
		public void TestApplyLinearImpulse()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyLinearImpulse(Vector2D.Zero);
		}

		[Test]
		public void TestApplyAngularImpulse()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyAngularImpulse(10.0f);
		}

		[Test]
		public void TestApplyTorque()
		{
			var body = physics.CreateRectangle(new Size(45.0f, 45.0f));
			Assert.IsNotNull(body);
			body.ApplyTorque(10.0f);
		}

		[Test]
		public void TestEmptyRectangleShapeNotAllowed()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => physics.CreateRectangle(Size.Zero));
		}
	}
}