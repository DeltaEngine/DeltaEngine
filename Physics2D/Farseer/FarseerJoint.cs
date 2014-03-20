using FarseerPhysics.Dynamics.Joints;

namespace DeltaEngine.Physics2D.Farseer
{
	/// <summary>
	/// Implements the Farseer flavor of physics joints
	/// </summary>
	internal sealed class FarseerJoint : PhysicsJoint
	{
		public FarseerJoint(Joint joint, PhysicsBody bodyA, PhysicsBody bodyB)
			: base(bodyA, bodyB)
		{
			this.joint = joint;
		}

		private readonly Joint joint;

		public override float MotorSpeed
		{
			get
			{
				var lineJoint = joint as PrismaticJoint;
				return lineJoint != null ? lineJoint.MotorSpeed : 0.0f;
			}
			set
			{
				var lineJoint = joint as PrismaticJoint;
				if (lineJoint != null)
					lineJoint.MotorSpeed = value;
			}
		}

		public override float MaxMotorTorque
		{
			get
			{
				var lineJoint = joint as PrismaticJoint;
				return lineJoint != null ? lineJoint.MaxMotorForce : 0.0f;
			}
			set
			{
				var lineJoint = joint as PrismaticJoint;
				if (lineJoint != null)
					lineJoint.MaxMotorForce = value;
			}
		}

		public override bool MotorEnabled
		{
			get
			{
				var lineJoint = joint as PrismaticJoint;
				return lineJoint != null && lineJoint.MotorEnabled;
			}
			set
			{
				var lineJoint = joint as PrismaticJoint;
				if (lineJoint != null)
					lineJoint.MotorEnabled = value;
			}
		}

		public override float Frequency
		{
			get
			{
				var lineJoint = joint as PrismaticJoint;
				return lineJoint != null ? lineJoint.MotorImpulse : 0.0f;
			}
			set
			{
				var lineJoint = joint as PrismaticJoint;
				if (lineJoint != null)
					lineJoint.MotorImpulse = value;
			}
		}
	}
}