using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.ScreenSpaces
{
	/// <summary>
	/// Converts to and from Pixel space. https://deltaengine.fogbugz.com/default.asp?W101
	/// </summary>
	public class PixelScreenSpace : ScreenSpace
	{
		public PixelScreenSpace(Window window)
			: base(window) {}

		protected override void Update(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			RaiseViewportSizeChanged();
		}

		public override Vector2D ToPixelSpace(Vector2D currentScreenSpacePosition)
		{
			return currentScreenSpacePosition;
		}

		public override Size ToPixelSpace(Size currentScreenSpaceSize)
		{
			return currentScreenSpaceSize;
		}

		public override Vector2D FromPixelSpace(Vector2D pixelPosition)
		{
			return pixelPosition;
		}

		public override Size FromPixelSpace(Size pixelSize)
		{
			return pixelSize;
		}

		public override Vector2D TopLeft
		{
			get { return Vector2D.Zero; }
		}
		public override Vector2D BottomRight
		{
			get { return viewportPixelSize; }
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
			get { return viewportPixelSize.Width; }
		}
		public override float Bottom
		{
			get { return viewportPixelSize.Height; }
		}

		public override Vector2D GetInnerPosition(Vector2D relativePosition)
		{
			return (Size)relativePosition * viewportPixelSize;
		}
	}
}