using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering2D
{
	/// <summary>
	/// Basis of all 2D entity objects to render like lines, sprites etc.
	/// </summary>
	public class Entity2D : DrawableEntity
	{
		protected Entity2D() {}

		public Entity2D(Rectangle drawArea)
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			LastDrawArea = DrawArea = drawArea;
		}

		public virtual Rectangle DrawArea
		{
			get { return drawArea; }
			set
			{
				drawArea = value;
				OnPositionChanged();
			}
		}

		private Rectangle drawArea;
		protected virtual void OnPositionChanged() {}

		public Rectangle LastDrawArea { get; set; }

		public Vector2D TopLeft
		{
			get { return DrawArea.TopLeft; }
			set { DrawArea = new Rectangle(value, DrawArea.Size); }
		}

		public Vector2D Center
		{
			get { return DrawArea.Center; }
			set { DrawArea = Rectangle.FromCenter(value, DrawArea.Size); }
		}

		public Size Size
		{
			get { return DrawArea.Size; }
			set { DrawArea = Rectangle.FromCenter(DrawArea.Center, value); }
		}

		public Color Color
		{
			get { return base.Contains<Color>() ? base.Get<Color>() : DefaultColor; }
			set { base.Set(value); }
		}

		public static readonly Color DefaultColor = Color.White;

		public float Alpha
		{
			get { return Color.AlphaValue; }
			set { Color = new Color(Color, value); }
		}

		public float Rotation
		{
			get { return base.Contains<float>() ? base.Get<float>() : DefaultRotation; }
			set
			{
				base.Set(value);
				OnRotationChanged();
			}
		}

		protected virtual void OnRotationChanged() {}

		private const float DefaultRotation = 0.0f;

		public Vector2D RotationCenter
		{
			get { return base.Contains<Vector2D>() ? base.Get<Vector2D>() : Get<Rectangle>().Center; }
			set { base.Set(value); }
		}

		protected override void NextUpdateStarted()
		{
			DidFootprintChange = LastDrawArea != DrawArea || GetLastRotation() != Rotation;
			LastDrawArea = DrawArea;
			base.NextUpdateStarted();
		}

		public bool DidFootprintChange { get; private set; }

		private float GetLastRotation()
		{
			object lastTickFloat = lastTickLerpComponents.Find(component => component is float);
			return lastTickFloat == null ? DefaultRotation : (float)lastTickFloat;
		}

		private Vector2D GetLastRotationCenter()
		{
			object lastTickPoint = lastTickLerpComponents.Find(component => component is Vector2D);
			return lastTickPoint == null ? LastDrawArea.Center : (Vector2D)lastTickPoint;
		}

		protected internal override List<object> GetComponentsForSaving()
		{
			var componentsForSaving = new List<object> { DrawArea, IsVisible };
			if (Color != DefaultColor)
				componentsForSaving.Add(Color);
			if (Rotation != DefaultRotation)
				componentsForSaving.Add(Rotation);
			if (RotationCenter != DrawArea.Center)
				componentsForSaving.Add(RotationCenter);
			foreach (var component in base.GetComponentsForSaving())
				if (!(component is Color) && !(component is float) && !(component is Vector2D))
					componentsForSaving.Add(component);
			return componentsForSaving;
		}

		public List<object> GetComponentsForEditing()
		{
			var componentsForEditing = GetComponentsForSaving();
			if (Color == DefaultColor)
				componentsForEditing.Add(Color);
			if (Rotation == DefaultRotation)
				componentsForEditing.Add(Rotation);
			if (RotationCenter == DrawArea.Center)
				componentsForEditing.Add(RotationCenter);
			return componentsForEditing;
		}

		public override T Get<T>()
		{
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(Rectangle))
				return (T)(object)LastDrawArea.Lerp(DrawArea, EntitiesRunner.CurrentDrawInterpolation);
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(Color))
				return (T)(object)LastColor.Lerp(Color, EntitiesRunner.CurrentDrawInterpolation);
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(float))
				return (T)(object)GetLastRotation().Lerp(Rotation, EntitiesRunner.CurrentDrawInterpolation);
			if (EntitiesRunner.Current.State == UpdateDrawState.Draw && typeof(T) == typeof(Vector2D))
				return (T)(object)GetLastRotationCenter().Lerp(RotationCenter,
					EntitiesRunner.CurrentDrawInterpolation);
			if (typeof(T) == typeof(Rectangle))
				return (T)(object)DrawArea;
			if (typeof(T) == typeof(Color))
				return (T)(object)Color;
			if (typeof(T) == typeof(float))
				return (T)(object)Rotation;
			if (typeof(T) == typeof(Vector2D))
				return (T)(object)RotationCenter;
			return base.Get<T>();
		}

		public Color LastColor
		{
			get
			{
				object lastTickColor = lastTickLerpComponents.Find(component => component is Color);
				return lastTickColor == null ? DefaultColor : (Color)lastTickColor;
			}
			set
			{
				for (int index = 0; index < lastTickLerpComponents.Count; index++)
					if (lastTickLerpComponents[index] is Color)
					{
						lastTickLerpComponents[index] = value;
						return;
					}
				lastTickLerpComponents.Add(value);
			}
		}

		public override sealed bool Contains<T>()
		{
			return typeof(T) == typeof(Rectangle) || typeof(T) == typeof(Color) ||
				typeof(T) == typeof(float) || typeof(T) == typeof(Vector2D) || typeof(T) == typeof(int) ||
				base.Contains<T>();
		}

		public override Entity Add<T>(T component)
		{
			if (typeof(T) == typeof(Rectangle) || typeof(T) == typeof(Color) ||
				typeof(T) == typeof(float) || typeof(T) == typeof(Vector2D) || typeof(T) == typeof(int))
				throw new ComponentOfTheSameTypeAddedMoreThanOnce();
			return base.Add(component);
		}

		public override void SetWithoutInterpolation<T>(T component)
		{
			if (typeof(T) == typeof(Rectangle))
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				DrawArea = (Rectangle)(object)component;
				LastDrawArea = DrawArea;
			}
			else if (typeof(T) == typeof(Color))
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				Color = (Color)(object)component;
				LastColor = Color;
			}
			else
				base.SetWithoutInterpolation(component);
		}

		public override void Set(object component)
		{
			if (component is Rectangle)
			{
				EntitiesRunner.Current.CheckIfInUpdateState();
				DrawArea = (Rectangle)component;
				return;
			}
			base.Set(component);
		}

		public bool RotatedDrawAreaContains(Vector2D position)
		{
			return
				DrawArea.Contains(Rotation == DefaultRotation
					? position : position.RotateAround(RotationCenter, -Rotation));
		}
	}
}