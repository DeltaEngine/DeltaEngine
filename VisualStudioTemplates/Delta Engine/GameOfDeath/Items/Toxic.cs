using DeltaEngine.Content;
using DeltaEngine.Multimedia;

namespace $safeprojectname$.Items
{
	/// <summary>
	/// Rabbits in the toxic area slowly die. Still effective due the huge range and long duration.
	/// </summary>
	public class Toxic : Item
	{
		public Toxic()
			: base(
				ContentLoader.Load<Material>("MaterialToxic"),
				ContentLoader.Load<Material>("MaterialToxicCloud"),
				ContentLoader.Load<Sound>("ToxicEffect"))
		{
			ItemType = ItemType.Toxic;
		}

		protected override float ImpactSize
		{
			get { return 0.15f; }
		}

		protected override float ImpactTime
		{
			get { return 5.0f; }
		}

		protected override float Damage
		{
			get { return 5; }
		}

		protected override float DamageInterval
		{
			get { return 0.1f; }
		}

		public override int Cost
		{
			get { return 20; }
		}
	}
}