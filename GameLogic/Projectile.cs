using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Physics3D;

namespace DeltaEngine.GameLogic
{
	public class Projectile : PhysicalEntity3D
	{
		public Projectile(Actor3D owner, Actor3D target, HierarchyObject3D missile)
		{
			Owner = owner;
			Position = owner.Position;
			Target = target;
			SetMissile(missile);
		}

		public Projectile(Vector3D position, Vector3D targetPosition, HierarchyObject3D missile,
			float mass = 0, float lifeTime = 0, Action onLifeTimeExceeded = null)
			: base(position, Quaternion.CreateLookAt(position, targetPosition, Vector3D.UnitZ), 
				mass, lifeTime)
		{
			SetMissile(missile);
			OnLifeTimeExceeded += onLifeTimeExceeded;
		}

		private void SetMissile(HierarchyObject3D missile)
		{
			Missile = missile;
			AddChild(missile);
		}

		public Actor Owner { get; private set; }
		public Actor Target { get; private set; }
		public HierarchyObject3D Missile { get; private set; }
	}
}