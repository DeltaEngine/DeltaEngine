using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Small default weapon to kill single rabbits before they multiply. Not effective to kill masses
	/// </summary>
	public class Mallet : Item
	{
		public Mallet()
			: base(
				ContentLoader.Load<Material>("MaterialMallet"), ContentLoader.Load<Material>("MaterialMalletEffect"),
				ContentLoader.Load<Sound>("MalletSwing"))
		{
			Start<RotateMallet>();
			ItemType = ItemType.Mallet;
		}

		public override void UpdatePosition(Vector2D newPosition)
		{
			base.UpdatePosition(newPosition + MalletOffset);
		}

		private static readonly Vector2D MalletOffset = new Vector2D(0.035f, 0.005f);

		protected override float ImpactSize
		{
			get { return 0.035f; }
		}

		protected override float ImpactTime
		{
			get { return 0.6f; }
		}

		protected override float Damage
		{
			get { return DefaultDamage; }
		}

		internal const float DefaultDamage = 3.3f;

		protected override float DamageInterval
		{
			get { return 0.0f; }
		}

		public override int Cost
		{
			get { return 1; }
		}

		public override ItemEffect CreateEffect(Vector2D position)
		{
			if (Rotation < 0)
				return null;

			Rotation = -60;
			return base.CreateEffect(position);
		}
	}
}