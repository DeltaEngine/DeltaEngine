using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Each item displays an fading out effect once applied.
	/// </summary>
	public class ItemEffect : FadeSprite
	{
		public ItemEffect(Material material, Rectangle drawArea, float duration)
			: base(material, drawArea, duration)
		{
			RenderLayer = (int)RenderLayers.Items;
			Add(new Damage { Interval = 0.25f });
			Start<DoDamage>();
		}

		public float DamageInterval
		{
			get { return Get<Damage>().Interval; }
			set { Get<Damage>().Interval = value; }
		}

		public Action DoDamage
		{
			get { return Get<Damage>().DoDamage; }
			set { Get<Damage>().DoDamage = value; }
		}

		public bool IsDamageOverTime { get; set; }
	}
}