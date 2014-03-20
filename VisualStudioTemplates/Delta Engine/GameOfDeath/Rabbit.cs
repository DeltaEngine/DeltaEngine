using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;

namespace $safeprojectname$
{
	/// <summary>
	/// Holds the rabbit sprite and healthbar
	/// </summary>
	public class Rabbit : Sprite
	{
		public Rabbit(Vector2D position, Size size)
			: base(ContentLoader.Load<Material>("MaterialRabbit"), Rectangle.FromCenter(position, size))
		{
			IsVisible = false;
			OriginalSize = size;
			RenderLayer = (int)RenderLayers.Rabbits;
			CreateRabbitHealthBar();
			Start<AlternatePositionInBoundingBox>();
		}

		public float CurrentHealth
		{
			get { return currentHealth; }
			set
			{
				currentHealth = value;
				UpdateHealthBar();
				if (currentHealth < MaxHealth)
					RabbitHealthBar.IsVisible = true;
				if (currentHealth <= 0)
					Stop<AlternatePositionInBoundingBox>();
			}
		}

		private float currentHealth;

		public float MaxHealth { get; set; }

		public Rectangle BoundingBox
		{
			get { return Rectangle.FromCenter(Center, DrawArea.Size * 0.05f); }
		}

		public Size OriginalSize { get; set; }

		private void CreateRabbitHealthBar()
		{
			var healthBar = new FilledRect(new Rectangle(), Color.Red) { IsVisible = false };
			healthBar.RenderLayer = RenderLayer + 1;
			Add(healthBar);
		}

		public void SetHealth(float initialHealth)
		{
			currentHealth = initialHealth;
			MaxHealth = initialHealth;
			IsVisible = true;
			UpdateHealthBar();
			Start<AlternatePositionInBoundingBox>();
		}

		public float HealthPercentage
		{
			get { return CurrentHealth / MaxHealth; }
		}

		public float Scale
		{
			set { DrawArea = Rectangle.FromCenter(DrawArea.Center, OriginalSize * value); }
		}

		public void DoDamage(float damage)
		{
			CurrentHealth -= damage;
		}

		public bool IsDead
		{
			get { return CurrentHealth <= 0; }
		}

		public FilledRect RabbitHealthBar
		{
			get { return Get<FilledRect>(); }
		}

		private void UpdateHealthBar()
		{
			RabbitHealthBar.DrawArea = Rectangle.FromCenter(Center.X, DrawArea.Top - 0.01f,
				HealthPercentage * DrawArea.Width * 0.5f, HealthBarHeight);
			var percentage = HealthPercentage;
			RabbitHealthBar.Color = percentage < 0.25f
				? HpBarRed : percentage < 0.5f ? HpBarYellow : HpBarGreen;
		}

		private const float HealthBarHeight = 10.0f / 1920;

		private static readonly Color HpBarRed = new Color(96, 0, 0, 128);
		private static readonly Color HpBarYellow = new Color(96, 96, 0, 128);
		private static readonly Color HpBarGreen = new Color(0, 96, 0, 128);

		private class AlternatePositionInBoundingBox : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var rabbit in entities.OfType<Rabbit>())
				{
					var randomX = rabbit.BoundingBox.Left.Lerp(rabbit.BoundingBox.Right,
						Randomizer.Current.Get());
					var randomY = rabbit.BoundingBox.Top.Lerp(rabbit.BoundingBox.Bottom,
						Randomizer.Current.Get());
					rabbit.Center = new Vector2D(randomX, randomY);
				}
			}
		}
	}
}