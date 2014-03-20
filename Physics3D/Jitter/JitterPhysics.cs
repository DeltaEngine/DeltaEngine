using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace DeltaEngine.Physics3D.Jitter
{
	/// <summary>
	/// Jitter physics implementation.
	/// </summary>
	public class JitterPhysics : Physics
	{
		public JitterPhysics()
		{
			collisionSystem = new CollisionSystemSAP();
			jitterWorld = new World(collisionSystem) { AllowDeactivation = true };
			jitterWorld.SetIterations(100, 100);
			jitterWorld.SetDampingFactors(0.95f, 0.95f);
			jitterWorld.SetInactivityThreshold(0.005f, 0.005f, 10);
			jitterWorld.Gravity = JitterDatatypesMapping.ConvertSlow(DefaultGravity);
			jitterWorld.Events.BodiesBeginCollide += BeginCollision;
			jitterWorld.Events.BodiesEndCollide += EndCollision;
			CreateGroundBody();
		}

		private readonly CollisionSystem collisionSystem;
		internal readonly World jitterWorld;

		public override bool IsShapeSupported(ShapeType shapeType)
		{
			switch (shapeType)
			{
			case ShapeType.Box:
			case ShapeType.Sphere:
			case ShapeType.Capsule:
			case ShapeType.Triangle:
			case ShapeType.Terrain:
			case ShapeType.Cone:
			case ShapeType.Cylinder:
				return true;
			}

			return false;
		}

		public override bool IsJointSupported(JointType jointType)
		{
			switch (jointType)
			{
			case JointType.FixedAngle:
			case JointType.SingleBodyPointOnLine:
			case JointType.PointOnLine:
			case JointType.PointOnPoint:
			case JointType.PointPointDistance:
			case JointType.Hinge:
			case JointType.Prismatic:
				return true;
			}

			return false;
		}

		public override void SetGroundPlane(bool enable, float height)
		{
			RigidBody jitterRigidBody = ((JitterBody)groundBody).Body;
			jitterRigidBody.Position = new JVector(0, 0, height - (PlaneHeight * 0.5f));
			if (enable && !jitterWorld.RigidBodies.Contains(jitterRigidBody))
				jitterWorld.AddBody(jitterRigidBody);
			else if (!enable && jitterWorld.RigidBodies.Contains(jitterRigidBody))
				jitterWorld.RemoveBody(jitterRigidBody);
		}

		private const float PlaneHeight = 1.0f;

		private void CreateGroundBody()
		{
			var jitterGroundBody = new RigidBody(new BoxShape(20000.0f, 20000.0f, PlaneHeight))
			{
				IsStatic = true,
				Material = { KineticFriction = 0.0f }
			};
			groundBody = new JitterBody(jitterGroundBody);
		}

		private static void BeginCollision(RigidBody rigidBody1, RigidBody rigidBody2)
		{
			var jitterBody1 = (JitterBody)rigidBody1.Tag;
			var jitterBody2 = (JitterBody)rigidBody2.Tag;
			jitterBody1.FireCollisionBegin(jitterBody2);
			jitterBody2.FireCollisionBegin(jitterBody1);
		}

		private static void EndCollision(RigidBody rigidBody1, RigidBody rigidBody2)
		{
			var jitterBody1 = (JitterBody)rigidBody1.Tag;
			var jitterBody2 = (JitterBody)rigidBody2.Tag;
			jitterBody1.FireCollisionEnd(jitterBody2);
			jitterBody2.FireCollisionEnd(jitterBody1);
		}

		public override PhysicsBody CreateBody(PhysicsShape shape, Vector3D initialPosition, float mass,
			float restitution)
		{
			PhysicsBody body = new JitterBody(this, shape, initialPosition, mass, restitution);
			bodies.Add(body);
			if (float.IsNaN(body.Position.X) || float.IsNaN(body.AngularVelocity.X) ||
				float.IsNaN(body.BoundingBox.Min.X) || float.IsNaN(body.BoundingBox.Max.X) ||
				float.IsNaN(body.GetOrientation().X))
				throw new PhysicsBodyIsNotSetUpProperlyMakeSurePositionVelocityAndBoundingBoxAreSet(body);
			return body;
		}

		public class PhysicsBodyIsNotSetUpProperlyMakeSurePositionVelocityAndBoundingBoxAreSet : Exception
		{
			public PhysicsBodyIsNotSetUpProperlyMakeSurePositionVelocityAndBoundingBoxAreSet(
				PhysicsBody body)
				: base(body.ToString()) {}
		}

		public override PhysicsJoint CreateJoint(JointType jointType, PhysicsBody bodyA,
			PhysicsBody bodyB, object[] args)
		{
			if (IsJointSupported(jointType) == false)
			{
				Logger.Warning("Current module does not support " + "the type of joint " + jointType);
				return null;
			}
			PhysicsJoint joint = new JitterJoint(this, jointType, bodyA, bodyB, args);
			joints.Add(joint);
			return joint;
		}

		protected override void RemoveBody(PhysicsBody body)
		{
			jitterWorld.RemoveBody((body as JitterBody).Body);
		}

		protected override void RemoveJoint(PhysicsJoint joint)
		{
			var jitterJoint = joint as JitterJoint;
			if (jitterJoint.Constraint != null)
				jitterWorld.RemoveConstraint(jitterJoint.Constraint);
			else
				jitterJoint.Joint.Deactivate();
		}

		public override RaycastResult DoRayCastIncludingGround(Ray ray)
		{
			var raycastResult = new JitterRaycastResult(collisionSystem, ray);
			if (!raycastResult.Found)
				return raycastResult;
			foreach (JitterBody body in bodies)
				if (body.Body == raycastResult.RigidBody)
					raycastResult.PhysicsBody = body;
			if (raycastResult.PhysicsBody == null)
				raycastResult.PhysicsBody = groundBody;
			return raycastResult;
		}

		public override RaycastResult DoRayCastExcludingGround(Ray ray)
		{
			var raycastResult = new JitterRaycastResult(collisionSystem, ray);
			if (raycastResult.Found)
				foreach (JitterBody body in bodies)
					if (body.Body == raycastResult.RigidBody)
						raycastResult.PhysicsBody = body;
			return raycastResult;
		}

		public override void SetGravity(Vector3D gravity)
		{
			jitterWorld.Gravity = JitterDatatypesMapping.Convert(ref gravity);
		}

		protected override double GetTotalPhysicsTime()
		{
			double total = 0;
			for (int i = 0; i < Entries; i++)
				total += jitterWorld.DebugTimes[i];
			return total;
		}

		private const int Entries = (int)World.DebugType.Num;

		protected override void Simulate(float timeStep)
		{
			jitterWorld.Step(timeStep, IsMultithreaded);
		}
	}
}