using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace Blocks
{
	/// <summary>
	/// Represents a brick within a block
	/// </summary>
	public class Brick : Sprite
	{
		public Brick(Material material, Vector2D offset, Orientation displayMode)
			: base(material, Rectangle.Zero)
		{
			Offset = offset;
			RenderLayer = (int)BlocksRenderLayer.Grid;
			this.displayMode = displayMode;
		}

		public Vector2D Offset;
		private readonly Orientation displayMode;

		public void UpdateDrawArea()
		{
			Vector2D newPoint;
			if (displayMode == Orientation.Landscape)
			{
				newPoint = OffsetLandscape + (Position - Vector2D.UnitY) * ZoomLandscape;
				DrawArea = NewDrawArea(newPoint, SizeLandscape);
			}
			else
			{
				newPoint = OffsetPortrait + (Position - Vector2D.UnitY) * ZoomPortrait;
				DrawArea = new Rectangle(newPoint, SizePortrait);
			}
		}

		private static Rectangle NewDrawArea(Vector2D point, Size size)
		{
			return new Rectangle(point, size);
		}

		public Vector2D Position
		{
			get { return TopLeftGridCoord + Offset; }
		}

		public Vector2D TopLeftGridCoord;
		public static readonly Vector2D OffsetLandscape = new Vector2D(0.38f, 0.385f);
		public static readonly Vector2D OffsetPortrait = new Vector2D(0.38f, 0.385f);
		public const float ZoomLandscape = 0.02f;
		public const float ZoomPortrait = 0.02f;
		private static readonly Size SizeLandscape = new Size(ZoomLandscape);
		private static readonly Size SizePortrait = new Size(ZoomPortrait);
	}
}