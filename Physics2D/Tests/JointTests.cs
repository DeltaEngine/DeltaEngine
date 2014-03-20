using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class JointTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			physics = Resolve<Physics>();
			body = physics.CreateCircle(3.0f);
		}

		private Physics physics;
		private PhysicsBody body;

		[Test]
		public void CreateFixedAngleJoint()
		{
			var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void CreateFixedAngleJointOnSingleBody()
		{
			var joint = physics.CreateFixedAngleJoint(body, (float)Math.PI / 3);
			Assert.AreEqual(joint.BodyA, body);
			Assert.AreEqual(joint.BodyB, body);
		}

		[Test]
		public void CreateAngleJoint()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(body, bodyB, (float)Math.PI / 2);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void CheckAngleJointBodies()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateAngleJoint(body, bodyB, (float)Math.PI / 2);
			Assert.AreEqual(joint.BodyA, body);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void CreateRevoluteJoint()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(body, bodyB, Vector2D.Zero);
			Assert.IsNotNull(joint);
		}

		[Test]
		public void CheckRevoluteJointBodies()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateRevoluteJoint(body, bodyB, Vector2D.Zero);
			Assert.AreEqual(joint.BodyA, body);
			Assert.AreEqual(joint.BodyB, bodyB);
		}

		[Test]
		public void ChangeLineJointMotorEnabled()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(body, bodyB, Vector2D.Zero);
			Assert.AreEqual(joint.MotorEnabled, false);
			joint.MotorEnabled = true;
			Assert.AreEqual(joint.MotorEnabled, true);
		}

		[Test]
		public void ChangeLineJointMaxMotorTorque()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(body, bodyB, Vector2D.Zero);
			joint.MaxMotorTorque = 1.0f;
			Assert.AreEqual(joint.MaxMotorTorque, 1.0f);
		}

		[Test]
		public void ChangeLineJointMotorSpeed()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(body, bodyB, Vector2D.Zero);
			joint.MotorSpeed = 4.0f;
			Assert.AreEqual(joint.MotorSpeed, 4.0f);
		}

		[Test]
		public void ChangeLineJointFrequency()
		{
			var bodyB = physics.CreateCircle(3.0f);
			var joint = physics.CreateLineJoint(body, bodyB, Vector2D.Zero);
			joint.Frequency = 1.0f;
			Assert.AreEqual(joint.Frequency, 1.0f);
		}
	}
}