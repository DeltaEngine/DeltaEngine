using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.Dynamics.Joints;
using Jitter.LinearMath;

namespace DeltaEngine.Physics3D.Jitter
{
	/// <summary>
	/// Jitter joint implementation supporting all JointTypes
	/// </summary>
	internal class JitterJoint : PhysicsJoint
	{
		public JitterJoint(JitterPhysics physicsManager, JointType jointType, PhysicsBody bodyA,
			PhysicsBody bodyB, object[] args)
			: base(jointType, bodyA, bodyB, args)
		{
			CreateJoint(physicsManager);
			if (Constraint != null)
				physicsManager.jitterWorld.AddConstraint(Constraint);
			if (Joint != null)
				Joint.Activate();
		}

		public Constraint Constraint { get; private set; }
		public Joint Joint { get; private set; }

		private void CreateJoint(JitterPhysics physicsManager)
		{
			if (JointType == JointType.FixedAngle)
				CreateFixedAngleJoint();
			if (JointType == JointType.PointOnLine)
				CreatePointOnLineJoint();
			if (JointType == JointType.PointOnPoint)
				CreatePointOnPointJoint();
			if (JointType == JointType.PointPointDistance)
				CreatePointPointDistanceJoint();
			if (JointType == JointType.Hinge)
				CreateHingeJoint(physicsManager);
			if (JointType == JointType.Prismatic)
				CreatePrismaticJoint(physicsManager);
			if (JointType == JointType.SingleBodyPointOnLine)
				CreateSingleBodyPointOnLineJoint();
		}

		private void CreateFixedAngleJoint()
		{
			if (RigidBodyB != null)
				Constraint = new FixedAngle(RigidBodyA, RigidBodyB);
			else
				Constraint = new global::Jitter.Dynamics.Constraints.SingleBody.FixedAngle(RigidBodyA);
		}

		private RigidBody RigidBodyB
		{
			get { return BodyB != null ? (BodyB as JitterBody).Body : null; }
		}

		private RigidBody RigidBodyA
		{
			get { return (BodyA as JitterBody).Body; }
		}

		private void CreatePointOnLineJoint()
		{
			JVector lineStartPointBody1;
			Vector3D tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.LineStartPointBody);
			JitterDatatypesMapping.Convert(ref tempVector, out lineStartPointBody1);
			JVector pointBody2;
			tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.PointBody);
			JitterDatatypesMapping.Convert(ref tempVector, out pointBody2);
			if (RigidBodyB != null)
				Constraint = new PointOnLine(RigidBodyA, RigidBodyB, lineStartPointBody1, pointBody2);
			else
				Logger.Warning("You're trying to create PointOnLine with second " +
					"body at null.Maybe you should create SingleBodyPointOnLine.");
		}

		private void CreatePointOnPointJoint()
		{
			JVector localAnchor;
			Vector3D tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.Anchor1);
			JitterDatatypesMapping.Convert(ref tempVector, out localAnchor);
			if (RigidBodyB != null)
				Constraint = new PointOnPoint(RigidBodyA, RigidBodyB, localAnchor);
			else
				Constraint = new global::Jitter.Dynamics.Constraints.SingleBody.PointOnPoint(RigidBodyA,
					localAnchor);
		}

		private void CreatePointPointDistanceJoint()
		{
			JVector anchor1;
			Vector3D tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.Anchor1);
			JitterDatatypesMapping.Convert(ref tempVector, out anchor1);
			JVector anchor2;
			tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.Anchor2);
			JitterDatatypesMapping.Convert(ref tempVector, out anchor2);
			Constraint = new PointPointDistance(RigidBodyA, RigidBodyB, anchor1, anchor2);
		}

		private void CreateHingeJoint(JitterPhysics physicsManager)
		{
			JVector position;
			Vector3D tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.Position);
			JitterDatatypesMapping.Convert(ref tempVector, out position);
			JVector hingeAxis;
			tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.HingeAxis);
			JitterDatatypesMapping.Convert(ref tempVector, out hingeAxis);
			Joint = new HingeJoint(physicsManager.jitterWorld, RigidBodyA, RigidBodyB, position,
				hingeAxis);
		}

		private void CreatePrismaticJoint(JitterPhysics physicsManager)
		{
			Joint = new PrismaticJoint(physicsManager.jitterWorld, RigidBodyA, RigidBodyB,
				ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
					PropertyType.MinimumDistance),
				ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
					PropertyType.MaximumDistance));
			float minimumSoftness = ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
				PropertyType.MinimumSoftness);
			float maximumSoftness = ArrayExtensions.GetWithDefault<PropertyType, float>(Properties,
				PropertyType.MaximumSoftness);
			(Joint as PrismaticJoint).MaximumDistanceConstraint.Softness = maximumSoftness;
			(Joint as PrismaticJoint).MinimumDistanceConstraint.Softness = minimumSoftness;
		}

		private void CreateSingleBodyPointOnLineJoint()
		{
			JVector anchor;
			Vector3D tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.Anchor1);
			JitterDatatypesMapping.Convert(ref tempVector, out anchor);
			JVector lineDirection;
			tempVector = ArrayExtensions.GetWithDefault<PropertyType, Vector3D>(Properties,
				PropertyType.LineDirection);
			JitterDatatypesMapping.Convert(ref tempVector, out lineDirection);
			Constraint = new global::Jitter.Dynamics.Constraints.SingleBody.PointOnLine(RigidBodyA,
				anchor, lineDirection);
		}

		/// <summary>
		/// Defines how big the applied impulses can get.
		/// </summary>
		public override float Softness
		{
			get { return base.Softness; }
			set
			{
				if (Constraint is FixedAngle)
					(Constraint as FixedAngle).Softness = value;
				if (Constraint is PointOnLine)
					(Constraint as PointOnLine).Softness = value;
				if (Constraint is PointOnPoint)
					(Constraint as PointOnPoint).Softness = value;
				if (Constraint is global::Jitter.Dynamics.Constraints.SingleBody.PointOnPoint)
					(Constraint as global::Jitter.Dynamics.Constraints.SingleBody.PointOnPoint).Softness =
						value;
				base.Softness = value;
			}
		}

		public override Vector3D Anchor1
		{
			get { return base.Anchor1; }
			set
			{
				if (Constraint is PointPointDistance)
					(Constraint as PointPointDistance).LocalAnchor1 = JitterDatatypesMapping.Convert(ref value);
				if (Constraint is global::Jitter.Dynamics.Constraints.SingleBody.PointOnPoint)
					(Constraint as global::Jitter.Dynamics.Constraints.SingleBody.PointOnPoint).Anchor =
						JitterDatatypesMapping.Convert(ref value);
				base.Anchor1 = value;
			}
		}

		public override Vector3D Anchor2
		{
			get { return base.Anchor2; }
			set
			{
				if (Constraint is PointPointDistance)
					(Constraint as PointPointDistance).LocalAnchor2 = JitterDatatypesMapping.Convert(ref value);
				base.Anchor2 = value;
			}
		}
	}
}