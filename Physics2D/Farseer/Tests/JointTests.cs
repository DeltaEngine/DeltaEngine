using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Farseer.Tests
{
	public class JointTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			physics = new FarseerPhysics();
		}

		private Physics physics;

		[Test]
		public void TestFixedAngleJoint()
		{
			var body = physics.CreateCircle(3.0f);
			var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestFixedAngleJointSameBodies()
		{
			var body = physics.CreateCircle(3.0f);
			var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.AreEqual(joint.BodyA, body);
			Assert.AreEqual(joint.BodyB, body);
		}

		[Test]
		public void TestAngleJoint()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestAngleJointBodiesEqual()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(bodyA, bodyB, (float)Math.PI / 2);
			Assert.AreEqual(joint.BodyA, bodyA);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void TestRevoluteJoint()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Vector2D.Zero);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void TestRevoluteJointBodiesEqual()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(bodyA, bodyB, Vector2D.Zero);
			Assert.AreEqual(joint.BodyA, bodyA);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void TestLineJointMotorEnabled()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Vector2D.Zero);
			Assert.AreEqual(joint.MotorEnabled, false);
			joint.MotorEnabled = true;
			Assert.AreEqual(joint.MotorEnabled, true);
		}

		[Test]
		public void TestLineJointMaxMotorTorque()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Vector2D.Zero);
			joint.MaxMotorTorque = 1.0f;
			Assert.AreEqual(joint.MaxMotorTorque, 1.0f);
		}

		[Test]
		public void TestLineJointMotorSpeed()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Vector2D.Zero);
			joint.MotorSpeed = 4.0f;
			Assert.AreEqual(joint.MotorSpeed, 4.0f);
		}

		[Test]
		public void TestLineJointFrequency()
		{
			var bodyA = physics.CreateCircle(3.0f);
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(bodyA, bodyB, Vector2D.Zero);
			joint.Frequency = 1.0f;
			Assert.AreEqual(joint.Frequency, 1.0f);
		}
	}
}