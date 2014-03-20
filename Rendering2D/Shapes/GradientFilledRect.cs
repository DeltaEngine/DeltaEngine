using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	public class GradientFilledRect : Entity2D
	{
		public GradientFilledRect(Rectangle drawArea, Color startColor, Color finalColor)
			: base(drawArea)
		{
			Color = startColor;
			FinalColor = finalColor;
			UpdateCorners();
			OnDraw<GradientRectRenderer>();
		}

		public readonly List<Vector2D> Points = new List<Vector2D>();
		public Color FinalColor { get; private set; }

		private void UpdateCorners()
		{
			Points.Clear();
			Points.AddRange(DrawArea.GetRotatedRectangleCorners(RotationCenter, Rotation));
		}

		protected override void NextUpdateStarted()
		{
			base.NextUpdateStarted();
			if (DidFootprintChange)
				UpdateCorners();
		}
	}
}