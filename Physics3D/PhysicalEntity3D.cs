using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Physics3D
{
	public class PhysicalEntity3D : HierarchyEntity3D, Updateable
	{
		public PhysicalEntity3D()
			: base(Vector3D.Zero, Quaternion.Identity)
		{
			SetDefaults();
		}

		public PhysicalEntity3D(Vector3D position, Quaternion rotation, float mass = 1,
			float lifeTime = 0)
			: base(position, rotation)
		{
			SetDefaults();
			Mass = mass;
			LifeTime = lifeTime;
		}

		private void SetDefaults()
		{
			Elapsed = 0;
			Mass = 1;
		}

		public Vector3D Velocity { get; set; }
		public Vector3D RotationAxis { get; set; }
		public float RotationSpeed { get; set; }

		public float Elapsed { get; set; }
		public float LifeTime { get; set; }

		public float Mass { get; set; }

		public PhysicsBody PhysicsBody { get; set; }

		public virtual void Update()
		{
			Elapsed += Time.Delta;
			if (LifeTime != 0 && Elapsed > LifeTime)
				ExceededLifeTime();
			if (PhysicsBody != null)
				UpdateWithPhysicsBody();
			else
				UpdateWithoutPhysicsBody();
		}

		private void UpdateWithPhysicsBody()
		{
			Position = PhysicsBody.Position;
			Velocity = PhysicsBody.LinearVelocity;
			RotationAxis = PhysicsBody.AngularVelocity;
			Orientation = PhysicsBody.GetOrientation();
			if (Orientation.X == float.NaN)
				throw new PhysicsBodyOrientationIsBrokenMakeSureAllValuesAreSet(PhysicsBody);
		}

		private class PhysicsBodyOrientationIsBrokenMakeSureAllValuesAreSet : Exception
		{
			public PhysicsBodyOrientationIsBrokenMakeSureAllValuesAreSet(PhysicsBody physicsBody)
				: base(physicsBody.ToString()) {}
		}

		private void UpdateWithoutPhysicsBody()
		{
			if (Orientation.Equals(Quaternion.Identity))
				Position += Velocity;
			else
				Position += (Velocity.Transform(Orientation));
			if (RotationSpeed != 0)
				Orientation *= Quaternion.FromAxisAngle(RotationAxis, RotationSpeed * Time.Delta);
		}

		private void ExceededLifeTime()
		{
			if (OnLifeTimeExceeded != null)
				OnLifeTimeExceeded();
			Dispose();
		}

		public event Action OnLifeTimeExceeded;

		public override void Dispose()
		{
			OnLifeTimeExceeded = null;
			base.Dispose();
		}
	}
}