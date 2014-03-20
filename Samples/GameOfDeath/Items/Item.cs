using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;

namespace GameOfDeath.Items
{
	/// <summary>
	/// Base for all items, manages the most important values, which need to be implemented.
	/// </summary>
	public abstract class Item : Sprite
	{
		protected Item(Material material, Material effectMaterial, Sound soundEffect)
			: base(material, Rectangle.FromCenter(Vector2D.Half, Size.Zero))
		{
			this.effectMaterial = effectMaterial;
			this.soundEffect = soundEffect;
			RenderLayer = (int)RenderLayers.Items + 1;
		}

		private readonly Material effectMaterial;
		private readonly Sound soundEffect;
		public ItemType ItemType { get; protected set; }

		public virtual void UpdatePosition(Vector2D newPosition)
		{
			DrawArea = Rectangle.FromCenter(newPosition,
				Material.DiffuseMap.PixelSize / UserInterface.QuadraticFullscreenSize);
		}

		protected abstract float ImpactSize { get; }
		protected abstract float ImpactTime { get; }
		protected abstract float Damage { get; }
		protected abstract float DamageInterval { get; }
		public abstract int Cost { get; }

		public virtual ItemEffect CreateEffect(Vector2D position)
		{
			soundEffect.Play();
			var size = new Size(ImpactSize * 2);
			if (effectMaterial == null)
				size.Width *= DrawArea.Size.AspectRatio;

			return new ItemEffect(effectMaterial ?? Material, Rectangle.FromCenter(position, size),
				ImpactTime)
			{
				DamageInterval = DamageInterval,
				DoDamage = () => DoDamage(position, ImpactSize, Damage),
				IsDamageOverTime = ItemType != ItemType.Mallet
			};
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				DoDamage = null;
				base.IsActive = value;
			}
		}

		public Action<Vector2D, float, float> DoDamage;
	}
}