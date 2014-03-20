using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Physics2D
{
	public class PhysicalEntity2D : HierarchyEntity2D, Updateable
	{
		public PhysicalEntity2D(Physics physics, Rectangle drawArea, Vector2D impulse,
			float lifeTime = 0)
			: base(drawArea)
		{
			LifeTime = lifeTime;
			ElapsedTime = 0;
			AddNewPhysicsBody(physics, drawArea, impulse);
			Start<AffixToPhysics2D>();
		}

		public float LifeTime { get; set; }

		public float ElapsedTime { get; private set; }

		private void AddNewPhysicsBody(Physics physics, Rectangle area, Vector2D impulse)
		{
			var physicsBody = physics.CreateRectangle(area.Size);
			physicsBody.Position = area.Center;
			physicsBody.Rotation = impulse.RotationTo(Vector2D.UnitX);
			physicsBody.IsStatic = false;
			physicsBody.ApplyLinearImpulse(impulse);
			Add(physicsBody);
		}

		public void Update()
		{
			ElapsedTime += Time.Delta;
			//ncrunch: no coverage start
			if (LifeTime != 0 && ElapsedTime > LifeTime)
				ExceededLifeTime();
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