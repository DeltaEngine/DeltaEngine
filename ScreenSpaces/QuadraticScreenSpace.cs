using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.ScreenSpaces
{
	/// <summary>
	/// Converts to and from quadratic space. https://deltaengine.net/Learn/ScreenSpace
	/// </summary>
	public class QuadraticScreenSpace : ScreenSpace
	{
		public QuadraticScreenSpace(Window window)
			: base(window)
		{
			CalculateScalesAndOffsets();
		}

		private void CalculateScalesAndOffsets()
		{
			quadraticToPixelScale = CalculateToPixelScale();
			quadraticToPixelOffset = CalculateToPixelOffset();
			pixelToQuadraticScale = CalculateToQuadraticScale();
			pixelToQuadraticOffset = CalculateToQuadraticOffset();
		}

		private Size quadraticToPixelScale;
		private Vector2D quadraticToPixelOffset;
		private Size pixelToQuadraticScale;
		private Vector2D pixelToQuadraticOffset;

		private Size CalculateToPixelScale()
		{
			Size scale = viewportPixelSize;
			if (viewportPixelSize.AspectRatio < 1.0f)
				scale.Width *= 1.0f / viewportPixelSize.AspectRatio;
			else if (viewportPixelSize.AspectRatio > 1.0f)
				scale.Height *= viewportPixelSize.AspectRatio;
			return scale;
		}

		private Vector2D CalculateToPixelOffset()
		{
			Vector2D offset = Vector2D.Zero;
			if (viewportPixelSize.AspectRatio < 1.0f)
				offset.X = (viewportPixelSize.Width - quadraticToPixelScale.Width) * 0.5f;
			else
				offset.Y = (viewportPixelSize.Height - quadraticToPixelScale.Height) * 0.5f;
			return offset;
		}

		private Size CalculateToQuadraticScale()
		{
			return 1.0f / quadraticToPixelScale;
		}

		private Vector2D CalculateToQuadraticOffset()
		{
			return new Vector2D(-quadraticToPixelOffset.X / quadraticToPixelScale.Width,
				-quadraticToPixelOffset.Y / quadraticToPixelScale.Height);
		}

		protected override void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			CalculateScalesAndOffsets();
			RaiseViewportSizeChanged();
		}

		public override Vector2D FromPixelSpace(Vector2D pixelPosition)
		{
			return new Vector2D(pixelToQuadraticScale.Width * pixelPosition.X + pixelToQuadraticOffset.X,
				pixelToQuadraticScale.Height * pixelPosition.Y + pixelToQuadraticOffset.Y);
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return pixelToQuadraticScale * pixelSize;
		}

		public override Vector2D ToPixelSpace(Vector2D currentScreenSpacePosition)
		{
			return new Vector2D(
				quadraticToPixelScale.Width * currentScreenSpacePosition.X + quadraticToPixelOffset.X,
				quadraticToPixelScale.Height * currentScreenSpacePosition.Y + quadraticToPixelOffset.Y);
		}

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return quadraticToPixelScale * currentScreenSpaceSize;
		}

		public override Vector2D TopLeft
		{
			get { return pixelToQuadraticOffset; }
		}

		public override Vector2D BottomRight
		{
			get { return new Vector2D(1 - pixelToQuadraticOffset.X, 1 - pixelToQuadraticOffset.Y); }
		}

		public override float Left
		{
			get { return pixelToQuadraticOffset.X; }
		}

		public override float Top
		{
			get { return pixelToQuadraticOffset.Y; }
		}

		public override float Right
		{
			get { return 1 - pixelToQuadraticOffset.X; }
		}

		public override float Bottom
		{
			get { return 1 - pixelToQuadraticOffset.Y; }
		}

		public override Vector2D GetInnerPosition(Vector2D relativePosition)
		{
			return new Vector2D(Left + (Right - Left) * relativePosition.X,
				Top + (Bottom - Top) * relativePosition.Y);
		}
	}
}