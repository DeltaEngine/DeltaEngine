using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics3D
{
	/// <summary>
	///  Represents a joint that attaches two bodies together.
	/// </summary>
	public abstract class PhysicsJoint
	{
		protected PhysicsJoint(JointType jointType, PhysicsBody bodyA, PhysicsBody bodyB,
			object[] args)
		{
			JointType = jointType;
			BodyA = bodyA;
			BodyB = bodyB;
			//PhysicsJoint: 'args' presumably populate Properties
			Properties = new Dictionary<PropertyType, object>();
		}

		public JointType JointType { get; private set; }
		public PhysicsBody BodyA { get; private set; }
		public PhysicsBody BodyB { get; private set; }
		public Dictionary<PropertyType, object> Properties { get; private set; }

		public virtual float Softness { get; set; }
		public virtual Vector3D Anchor1 { get; set; }
		public virtual Vector3D Anchor2 { get; set; }

		public enum PropertyType : byte
		{
			TargetAngle,
			Anchor1,
			Anchor2,
			LineDirection,
			LineStartPointBody,
			PointBody,
			Position,
			HingeAxis,
			MinimumDistance,
			MaximumDistance,
			MinimumSoftness,
			MaximumSoftness,
		}
	}
}