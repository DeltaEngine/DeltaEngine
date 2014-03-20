using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.ScreenSpaces
{
	/// <summary>
	/// Converts to and from Relative space. https://deltaengine.fogbugz.com/default.asp?W101
	/// </summary>
	public class RelativeScreenSpace : ScreenSpace
	{
		public RelativeScreenSpace(Window window)
			: base(window)
		{
			pixelToRelativeScale = 1.0f / window.ViewportPixelSize;
		}

		private Size pixelToRelativeScale;

		protected override void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			pixelToRelativeScale = 1.0f / viewportPixelSize;
			RaiseViewportSizeChanged();
		}

		public override Vector2D ToPixelSpace(Vector2D currentScreenSpacePosition)
		{
			return ToPixelSpace((Size)currentScreenSpacePosition);
		}

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return currentScreenSpaceSize * viewportPixelSize;
		}

		public override Vector2D FromPixelSpace(Vector2D pixelPosition)
		{
			return FromPixelSpace((Size)pixelPosition);
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return pixelSize * pixelToRelativeScale;
		}

		public override Vector2D TopLeft
		{
			get { return Vector2D.Zero; }
		}
		public override Vector2D BottomRight
		{
			get { return Vector2D.One; }
		}
		public override float Left
		{
			get { return 0.0f; }
		}
		public override float Top
		{
			get { return 0.0f; }
		}
		public override float Right
		{
			get { return 1.0f; }
		}
		public override float Bottom
		{
			get { return 1.0f; }
		}

		public override Vector2D GetInnerPosition(Vector2D relativePosition)
		{
			return relativePosition;
		}
	}
}