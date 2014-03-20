using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// 2D sprite to be rendered, which is an image or animation or sprite sheet used as an Entity2D.
	/// </summary>
	public class Sprite : Entity2D
	{
		protected Sprite()
			: base(Rectangle.Zero) {}

		public Sprite(string imageName, Rectangle drawArea)
			: this(new Material(ShaderFlags.Position2DColoredTextured, imageName), drawArea) {}

		public Sprite(string imageName, Vector2D position)
			: this(new Material(ShaderFlags.Position2DColoredTextured, imageName), position) {}

		public Sprite(Material material, Vector2D position)
			: this(material, Rectangle.FromCenter(position, material.MaterialRenderSize)) {}

		public Sprite(Material material, Rectangle drawArea)
			: base(drawArea)
		{
			Add(cachedMaterial = material);
			Add(cachedBlendMode =
				material.DiffuseMap != null ? material.DiffuseMap.BlendMode : BlendMode.Normal);
			if (material.DefaultColor != DefaultColor)
				Color = material.DefaultColor;
			OnDraw<SpriteRenderer>();
			if (material.Animation != null)
				Start<UpdateImageAnimation>();
			if (material.SpriteSheet != null)
				Start<UpdateSpriteSheetAnimation>();
			lastRenderingData = renderingData =
				Material.RenderingCalculator.GetUVAndDrawArea(Rectangle.One, drawArea);
			Start<UpdateRenderingCalculations>();
			IsPlaying = true;
		}

		internal RenderingData renderingData;
		internal RenderingData lastRenderingData;
		public bool IsPlaying { get; set; }

		public override void Set(object component)
		{
			if (component is RenderingData)
			{
				lastRenderingData = renderingData = (RenderingData)component;
				return;
			}
			if (component is Material)
				cachedMaterial = component as Material;
			if (component is BlendMode)
				cachedBlendMode = (BlendMode)component;
			base.Set(component);
		}

		public override sealed void SetWithoutInterpolation<T>(T component)
		{
			if (typeof(T) == typeof(RenderingData))
				lastRenderingData = renderingData = (RenderingData)(object)component;
			else
				base.SetWithoutInterpolation(component);
		}

		public override sealed Entity Add<T>(T component)
		{
			if (typeof(T) == typeof(RenderingData))
				throw new RenderingDataComponentAddingIsNotSupported();
			return base.Add(component);
		}

		public class RenderingDataComponentAddingIsNotSupported : Exception {}

		public override sealed T Get<T>()
		{
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw &&
				typeof(T) == typeof(RenderingData) && lastRenderingData != null)
				return (T)(object)lastRenderingData.Lerp(renderingData,
					EntitiesRunner.CurrentDrawInterpolation);
			if (typeof(T) == typeof(RenderingData))
				return (T)(object)renderingData;
			return base.Get<T>();
		}

		protected internal override sealed List<object> GetComponentsForSaving()
		{
			List<object> componentsForSaving = base.GetComponentsForSaving(); 
			componentsForSaving.Add(renderingData);
			return componentsForSaving;
		}

		protected override sealed void NextUpdateStarted()
		{
			lastRenderingData = renderingData;
			base.NextUpdateStarted();
		}

		public Rectangle UV
		{
			get { return renderingData.RequestedUserUV; }
			set
			{
				renderingData = Material.RenderingCalculator.GetUVAndDrawArea(value, DrawArea, FlipMode);
			}
		}

		public Rectangle LastUV
		{
			get { return lastRenderingData.RequestedUserUV; }
			set
			{
				lastRenderingData = Material.RenderingCalculator.GetUVAndDrawArea(value, LastDrawArea,
					FlipMode);
			}
		}

		public FlipMode FlipMode
		{
			get { return renderingData.FlipMode; }
			set 
			{ 
				lastRenderingData = renderingData = 
					Material.RenderingCalculator.GetUVAndDrawArea(UV, DrawArea, value); 
			}
		}

		public Material Material
		{
			get { return cachedMaterial; }
			set
			{
				Set(value);
				if (value.DiffuseMap != null)
					BlendMode = value.DiffuseMap.BlendMode;
			}
		}
		private Material cachedMaterial;

		public BlendMode BlendMode
		{
			get { return cachedBlendMode; }
			set { Set(value); }
		}
		private BlendMode cachedBlendMode;

		public void Reset()
		{
			CurrentFrame = 0;
			if (Material.Animation != null || Material.SpriteSheet != null)
				while (Elapsed >= Material.Duration)
					Elapsed -= Material.Duration;
			else
				Elapsed = 0.0f;
		}

		public float Elapsed { get; set; }
		public int CurrentFrame { get; set; }

		internal void InvokeAnimationEndedAndReset()
		{
			if (AnimationEnded != null)
				AnimationEnded();
			Reset();
		}

		public event Action AnimationEnded;
	}
}