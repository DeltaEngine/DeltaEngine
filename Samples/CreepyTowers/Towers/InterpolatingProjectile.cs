using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D;

namespace CreepyTowers.Towers
{
	public class InterpolatingProjectile : HierarchyEntity3D, Updateable
	{
		public InterpolatingProjectile(Actor3D owner, Actor3D target, HierarchyObject3D missile)
			: base(owner.Position)
		{
			elapsedTime = 0;
			Owner = owner;
			Target = target;
			Position = Owner.Position;
			Orientation = Quaternion.CreateLookAt(Owner.Position, Target.Position, Vector3D.UnitZ);
			timeTillImpact = Owner.Position.DistanceSquared(Target.Position) / ProjectileSpeed;
			AddChild(missile);
		}

		public Actor3D Owner { get; set; }
		public Actor3D Target { get; set; }

		private readonly float timeTillImpact;
		private const float ProjectileSpeed = 30.0f;

		public void Update()
		{
			elapsedTime += Time.Delta;
			Position = Owner.Position.Lerp(Target.Position, elapsedTime / timeTillImpact);
			if (elapsedTime >= timeTillImpact)
			{
				var tower = (Tower)Owner;
				((Creep)Target).ReceiveAttack(tower.Type, tower.GetStatValue("Power"));
				Dispose();
			}
		}

		private float elapsedTime;
	}
}