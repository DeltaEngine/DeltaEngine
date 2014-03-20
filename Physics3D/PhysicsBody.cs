using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics3D
{
	/// <summary>
	/// Represents a body which responds to physics
	/// </summary>
	public abstract class PhysicsBody
	{
		protected PhysicsBody(PhysicsShape shape)
		{
			Shape = shape;
		}

		public PhysicsShape Shape { get; private set; }
		
		//PhysicsBody Collision begin and end
		protected virtual void OnCollisionBegin(PhysicsBody other)
		{
			if (CollisionBegin != null)
				CollisionBegin(other);
		}

		public Action<PhysicsBody> CollisionBegin;

		protected virtual void OnCollisionEnd(PhysicsBody other)
		{
			if (CollisionEnd != null)
				CollisionEnd(other);
		}

		public Action<PhysicsBody> CollisionEnd;

		public abstract Vector3D Position { get; set; }
		public abstract Vector2D Position2D { get; }
		public abstract Matrix RotationMatrix { get; set; }
		public abstract Vector3D LinearVelocity { get; set; }
		public abstract Vector3D AngularVelocity { get; set; }
		public abstract float AngularVelocity2D { get; set; }
		public virtual float Mass { get; set; }
		public virtual float Restitution { get; set; }
		public abstract BoundingBox BoundingBox { get; }
		public abstract void ApplyForce(Vector3D force);
		public abstract void ApplyForce(Vector3D force, Vector3D position);
		public abstract void ApplyTorque(Vector3D torque);
		public abstract void ApplyLinearImpulse(Vector3D impulse);
		public abstract void ApplyLinearImpulse(Vector3D impulse, Vector3D position);
		public abstract void ApplyAngularImpulse(Vector3D impulse);
		protected abstract void SetIsStatic(bool value);
		protected abstract void SetIsActive(bool value);
		protected abstract void SetFriction(float value);

		public Quaternion GetOrientation()
		{
			return Quaternion.FromRotationMatrix(RotationMatrix);
		}
	}
}