using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Tests.ScreenSpaces
{
	/// <summary>
	/// Converts to and from Inverted space. https://deltaengine.fogbugz.com/default.asp?W101
	/// (0, 0) == BottomLeft, (1, 1) == TopRight
	/// </summary>
	internal class InvertedScreenSpace : ScreenSpace
	{
		public InvertedScreenSpace(Window window)
			: base(window)
		{
			pixelToRelativeScale = 1.0f / window.ViewportPixelSize;
		}

		private Size pixelToRelativeScale;

		protected override void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			pixelToRelativeScale = 1.0f / viewportPixelSize;
		}

		public override Vector2D ToPixelSpace(Vector2D currentScreenSpacePosition)
		{
			Vector2D position = Vector2D.Zero;
			position.X = currentScreenSpacePosition.X * viewportPixelSize.Width;
			position.Y = (1 - currentScreenSpacePosition.Y) * viewportPixelSize.Height;
			return position;
		}

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return currentScreenSpaceSize * viewportPixelSize;
		}

		public override Vector2D FromPixelSpace(Vector2D pixelPosition)
		{
			Vector2D position = Vector2D.Zero;
			position.X = pixelPosition.X * pixelToRelativeScale.Width;
			position.Y = 1 - pixelPosition.Y * pixelToRelativeScale.Height;
			return position;
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return pixelSize * pixelToRelativeScale;
		}

		public override Vector2D TopLeft
		{
			get { return Vector2D.UnitY; }
		}
		public override Vector2D BottomRight
		{
			get { return Vector2D.UnitX; }
		}
		public override float Left
		{
			get { return 0.0f; }
		}
		public override float Top
		{
			get { return 1.0f; }
		}
		public override float Right
		{
			get { return 1.0f; }
		}
		public override float Bottom
		{
			get { return 0.0f; }
		}

		public override Vector2D GetInnerPosition(Vector2D relativePosition)
		{
			Vector2D position = Vector2D.Zero;
			position.X = relativePosition.X;
			position.Y = 1 - relativePosition.Y;
			return position;
		}
	}
}