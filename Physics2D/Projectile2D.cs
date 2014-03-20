using DeltaEngine.Datatypes;

namespace DeltaEngine.Physics2D
{
	public class Projectile2D : PhysicalEntity2D
	{
		public Projectile2D(Physics physics, Vector2D impulse, Rectangle area, int damage)
			: base(physics, area, impulse)
		{
			Damage = damage;
		}

		public int Damage { get; private set; }
	}
}