using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes;

namespace $safeprojectname$
{
	/// <summary>
	/// Renders the static elements (background, grid, score) in landscape plus keeps track of and 
	/// renders the player score.
	/// </summary>
	public class UserInterfaceLandscape : Scene
	{
		public UserInterfaceLandscape(BlocksContent content)
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
			var material = new Material(ShaderFlags.Position2DColoredTextured, content.Prefix + "Grid");
			grid = new Sprite(material, GetGridDrawArea()) { RenderLayer = Background };
			Add(grid);
		}

		private Sprite grid;

		private static Rectangle GetGridDrawArea()
		{
			var left = Brick.OffsetLandscape.X + GridRenderLeftOffset;
			var top = Brick.OffsetLandscape.Y - Brick.ZoomLandscape + GridRenderTopOffset;
			const float Width = Grid.Width * Brick.ZoomLandscape + GridRenderWidthOffset;
			const float Height = (Grid.Height + 1) * Brick.ZoomLandscape + GridRenderHeightOffset;
			return new Rectangle(left, top, Width, Height);
		}

		private const float GridRenderLeftOffset = -0.009f;
		private const float GridRenderTopOffset = -0.009f;
		private const float GridRenderWidthOffset = 0.019f;
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
			var left = Brick.OffsetLandscape.X + GridRenderLeftOffset;
			var top = Brick.OffsetLandscape.Y - Brick.ZoomLandscape + ScoreRenderTopOffset;
			const float Width = Grid.Width * Brick.ZoomLandscape + GridRenderWidthOffset;
			var height = Width / size.AspectRatio;
			return new Rectangle(left, top, Width, height);
		}

		private const float ScoreRenderTopOffset = -0.135f;

		private void AddScore()
		{
			Text = new FontText(ContentLoader.Load<Font>("Verdana12"), "", scoreWindow.DrawArea)
			{
				RenderLayer = (int)BlocksRenderLayer.Foreground
			};
		}

		internal FontText Text { get; private set; }

		public void ResizeInterface()
		{
			grid.DrawArea = GetGridDrawArea();
			scoreWindow.DrawArea = GetScoreWindowDrawArea(scoreWindow.Material.DiffuseMap.PixelSize);
		}
	}
}