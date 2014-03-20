namespace DeltaEngine.Physics2D
{
	/// <summary>
	///  Represents a joint that attaches two bodies together.
	/// </summary>
	public abstract class PhysicsJoint
	{
		protected PhysicsJoint(PhysicsBody bodyA, PhysicsBody bodyB)
		{
			BodyA = bodyA;
			BodyB = bodyB;
		}

		public PhysicsBody BodyA { get; private set; }
		public PhysicsBody BodyB { get; private set; }
		public abstract float MotorSpeed { get; set; }
		public abstract float MaxMotorTorque { get; set; }
		public abstract bool MotorEnabled { get; set; }
		public abstract float Frequency { get; set; }
	}
}