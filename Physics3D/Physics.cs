using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Physics3D
{
	/// <summary>
	/// Main physics class used for performing simulations and creating bodies and shapes.
	/// </summary>
	public abstract class Physics : Entity, Updateable
	{
		public void Update()
		{
			Simulate(Time.Delta);
		}

		protected abstract void Simulate(float delta);

		protected void AddBody(PhysicsBody body)
		{
			bodies.Add(body);
		}

		protected readonly List<PhysicsBody> bodies = new List<PhysicsBody>();
		protected PhysicsBody groundBody;
		protected readonly List<PhysicsJoint> joints = new List<PhysicsJoint>();
		public bool IsMultithreaded { get; set; }
		public readonly Vector3D DefaultGravity = new Vector3D(0, 0, -9.81f);

		public abstract void SetGravity(Vector3D gravity);
		protected abstract double GetTotalPhysicsTime();
		public abstract PhysicsBody CreateBody(PhysicsShape shape, Vector3D initialPosition,
			float mass, float restitution);
		protected abstract void RemoveJoint(PhysicsJoint joint);
		protected abstract void RemoveBody(PhysicsBody body);
		public abstract RaycastResult DoRayCastIncludingGround(Ray ray);
		public abstract RaycastResult DoRayCastExcludingGround(Ray ray);
		public abstract bool IsShapeSupported(ShapeType shapeType);
		public abstract bool IsJointSupported(JointType jointType);
		public abstract void SetGroundPlane(bool enable, float height);
		public abstract PhysicsJoint CreateJoint(JointType jointType, PhysicsBody bodyA,
			PhysicsBody bodyB, object[] args);
	}
}