using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;

namespace Blocks
{
	/// <summary>
	/// Renders the static elements (background, grid, score) in portrait plus keeps track of and 
	/// renders the player score.
	/// </summary>
	public class UserInterfacePortrait : Scene
	{
		public UserInterfacePortrait(BlocksContent content)
		{
			this.content = content;
			AddBackground();
			AddGrid();
			AddScoreWindow();
			AddScore();
		}

		private readonly BlocksContent content;

		private void AddBackground()
		{
			SetViewportBackground(new Material(ShaderFlags.Position2DColoredTextured, content.Prefix + "Background"));
		}

		private const int Background = (int)BlocksRenderLayer.Background;

		private void AddGrid()
		{
			Add(new Sprite(content.Prefix + "Grid", GetGridDrawArea()) { RenderLayer = Background });
		}

		private static Rectangle GetGridDrawArea()
		{
			var left = Brick.OffsetPortrait.X + GridRenderLeftOffset;
			var top = Brick.OffsetPortrait.Y - Brick.ZoomPortrait + GridRenderTopOffset;
			return new Rectangle(left, top, Width, Height);
		}

		private const float GridRenderLeftOffset = -0.009f;
		private const float GridRenderTopOffset = -0.009f;
		private const float GridRenderWidthOffset = 0.019f;
		private const float Width = Grid.Width * Brick.ZoomPortrait + GridRenderWidthOffset;
		private const float Height = (Grid.Height + 1) * Brick.ZoomPortrait + GridRenderHeightOffset;
		private const float GridRenderHeightOffset = 0.018f;

		private void AddScoreWindow()
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured, content.Prefix + "ScoreWindow");
			scoreWindow = new Sprite(material, GetScoreWindowDrawArea(material.DiffuseMap.PixelSize));
			scoreWindow.RenderLayer = Background;
			Add(scoreWindow);
		}

		private Sprite scoreWindow;

		private static Rectangle GetScoreWindowDrawArea(Size size)
		{
			var left = Brick.OffsetPortrait.X + GridRenderLeftOffset;
			var top = Brick.OffsetPortrait.Y - Brick.ZoomPortrait + ScoreRenderTopOffset;
			var height = Width / size.AspectRatio;
			return new Rectangle(left, top, Width, height);
		}

		private void AddScore()
		{
			Text = new FontText(ContentLoader.Load<Font>("Verdana12"), "", scoreWindow.DrawArea)
			{
				RenderLayer = (int)BlocksRenderLayer.Foreground + 6
			};
		}

		internal FontText Text { get; private set; }

		private const float ScoreRenderTopOffset = -0.135f;
	}
}