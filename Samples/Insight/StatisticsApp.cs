using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace Insight
{
	public class StatisticsApp
	{
		public StatisticsApp(Window window)
		{
			window.BackgroundColor = Color.White;
			window.Title = "Insight";
			CreateAppMenu();
		}

		public void CreateAppMenu()
		{
			mainBg = new Material(ShaderFlags.Position2DColoredTextured, "MainBg");
			userStatsMaterial = new Material(ShaderFlags.Position2DColoredTextured, "ButtonUserStats");
			topLocationMaterial = new Material(ShaderFlags.Position2DColoredTextured, "ButtonTopLocation");
			topSegmentUserStatsMaterial = new Material(ShaderFlags.Position2DColoredTextured, "UserStatsBg");
			userStatButtonPressedMaterial = new Material(ShaderFlags.Position2DColoredTextured,
				"ButtonUserStatsPressed");
			topSegmentTopLocationMaterial = new Material(ShaderFlags.Position2DColoredTextured, "TopLocationBg");
			topLocationButtonPressedMaterial = new Material(ShaderFlags.Position2DColoredTextured,
				"ButtonTopLocationsPressed");
			topSegment = new Sprite(mainBg, Rectangle.Zero);
			userStats = new Sprite(userStatsMaterial, Rectangle.Zero);
			topLocation = new Sprite(topLocationMaterial, Rectangle.Zero);
			PositionControls();
			new Command(Command.Click, ButtonClick);
			ScreenSpace.Current.ViewportSizeChanged += PositionControls;
		}

		private Material mainBg;
		private Material userStatsMaterial;
		private Material topLocationMaterial;
		private Material topSegmentUserStatsMaterial;
		private Material userStatButtonPressedMaterial;
		private Material topSegmentTopLocationMaterial;
		private Material topLocationButtonPressedMaterial;
		private Sprite topSegment;
		private Sprite userStats;
		private Sprite topLocation;

		private void PositionControls()
		{
			var height = ScreenSpace.Current.Viewport.Height;
			var width = ScreenSpace.Current.Viewport.Width;
			var topBottomBorder = height * 0.02f;
			var rightLeftBorder = width * 0.025f;
			var top = ScreenSpace.Current.Viewport.Top;
		
			var drawArea = new Rectangle(0.5f - height * 0.2825f + rightLeftBorder,
				top + topBottomBorder, height * 0.53675f, height * 0.665f);
			topSegment.LastDrawArea = topSegment.DrawArea = drawArea;
			userStats.LastDrawArea =
				userStats.DrawArea =
					new Rectangle(drawArea.Left, drawArea.Bottom + topBottomBorder, height * 0.260f,
						height * 0.255f);
			topLocation.LastDrawArea =
				topLocation.DrawArea =
					new Rectangle(drawArea.Left + height * 0.260f + 0.9f * topBottomBorder,
						drawArea.Bottom + topBottomBorder, height * 0.260f, height * 0.255f);
		}

		private void ButtonClick(Vector2D position)
		{
			if (userStats.DrawArea.Contains(position))
				ClickUserStatButton();
			else if (topLocation.DrawArea.Contains(position))
				ClickTopLocationButton();
		}

		private void ClickUserStatButton()
		{
			topSegment.Material = topSegmentUserStatsMaterial;
			userStats.Material = userStatButtonPressedMaterial;
			topLocation.Material = topLocationMaterial;
		}

		private void ClickTopLocationButton()
		{
			topSegment.Material = topSegmentTopLocationMaterial;
			userStats.Material = userStatsMaterial;
			topLocation.Material = topLocationButtonPressedMaterial;
		}
	}
}