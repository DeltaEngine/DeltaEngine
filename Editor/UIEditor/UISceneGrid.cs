using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.UIEditor
{
	public class UISceneGrid
	{
		public UISceneGrid(UIEditorScene uiEditorScene)
		{
			GridHeight = 30;
			GridHeight = 30;
			this.uiEditorScene = uiEditorScene;
			DrawGrid();
		}

		private readonly UIEditorScene uiEditorScene;

		public void MoveGridOutlinePoints(Rectangle rect)
		{
			GridOutline[0].Points[0] = rect.TopLeft;
			GridOutline[0].Points[1] = new Vector2D(rect.TopLeft.X + rect.Width,
				rect.TopLeft.Y);
			GridOutline[1].Points[0] = rect.TopLeft;
			GridOutline[1].Points[1] = new Vector2D(rect.TopLeft.X,
				rect.TopLeft.Y + rect.Height);
			GridOutline[2].Points[0] = new Vector2D(rect.TopLeft.X,
				rect.TopLeft.Y + rect.Height);
			GridOutline[2].Points[1] = new Vector2D(rect.TopLeft.X + rect.Width,
				rect.TopLeft.Y + rect.Height);
			GridOutline[3].Points[0] = new Vector2D(rect.TopLeft.X + rect.Width,
				rect.TopLeft.Y);
			GridOutline[3].Points[1] = new Vector2D(rect.TopLeft.X + rect.Width,
				rect.TopLeft.Y + rect.Height);
		}

		public void DrawGrid()
		{
			foreach (Line2D line2D in LinesInGridList)
				line2D.IsActive = false;
			LinesInGridList.Clear();
			if (uiEditorScene.SceneResolution.Width <= 0 || uiEditorScene.SceneResolution.Height <= 0 ||
				GridWidth == 0 || GridHeight == 0 || !uiEditorScene.IsDrawingGrid)
				return;
			CreateGridWithRightResolution();
			UpdateRenderlayerGrid();
		}

		public List<Line2D> LinesInGridList = new List<Line2D>();
		public int GridHeight { get; set; }
		public int GridWidth { get; set; }

		private void CreateGridWithRightResolution()
		{
			var sceneSize = ScreenSpace.Current.FromPixelSpace(uiEditorScene.SceneResolution);
			var xOffset = 0.5f;
			var yOffset = 0.5f;
			var aspect = sceneSize.Width / sceneSize.Height;
			if (aspect > 1)
				yOffset = 1 / (2 * aspect);
			else if (aspect < 1)
				xOffset = aspect / 2;
			if (yOffset < xOffset)
				CreateLandScapeGrid(xOffset, sceneSize, yOffset, aspect);
			else
				CreatePortraitGrid(xOffset, sceneSize, aspect, yOffset);
		}

		private void CreateLandScapeGrid(float xOffset, Size sceneSize, float yOffset, float aspect)
		{
			var tileSize = ScreenSpace.Current.FromPixelSpace(new Size(GridWidth, GridHeight));
			for (int i = 0; i < uiEditorScene.SceneResolution.Width / GridWidth; i++)
				LinesInGridList.Add(
					new Line2D(
						new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width))),
							0.5f - yOffset),
						new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width))),
							1 - (0.5f - yOffset)), Color.Red));
			for (int j = 0; j < uiEditorScene.SceneResolution.Height / GridHeight; j++)
				LinesInGridList.Add(
					new Line2D(
						new Vector2D(0.5f - xOffset,
							(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)) / aspect)),
						new Vector2D(1 - (0.5f - xOffset),
							(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)) / aspect)), Color.Red));
		}

		private void CreatePortraitGrid(float xOffset, Size sceneSize, float aspect, float yOffset)
		{
			var tileSize = ScreenSpace.Current.FromPixelSpace(new Size(GridWidth, GridHeight));
			for (int i = 0; i < uiEditorScene.SceneResolution.Width / GridWidth; i++)
				LinesInGridList.Add(
					new Line2D(
						new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width)) * aspect),
							0.5f - yOffset),
						new Vector2D((0.5f - xOffset + i * (1 / (sceneSize.Width / tileSize.Width)) * aspect),
							1 - (0.5f - yOffset)), Color.Red));
			for (int j = 0; j < uiEditorScene.SceneResolution.Height / GridHeight; j++)
				LinesInGridList.Add(
					new Line2D(
						new Vector2D(0.5f - xOffset,
							(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)))),
						new Vector2D(1 - (0.5f - xOffset),
							(0.5f - yOffset + j * (1 / (sceneSize.Height / tileSize.Height)))), Color.Red));
		}


		public void CreateGritdOutline()
		{
			GridOutline.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutline.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutline.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
			GridOutline.Add(new Line2D(new Vector2D(0, 0), new Vector2D(0, 0), Color.Red));
		}

		public List<Line2D> GridOutline = new List<Line2D>();

		public void UpdateGridOutline(Size SceneResolution)
		{
			if (SceneResolution.Width <= 0 || SceneResolution.Height <= 0)
				return;
			var sceneSize = ScreenSpace.Current.FromPixelSpace(SceneResolution);
			var rectangle = CalculateNewTopLeft(sceneSize);
			MoveGridOutlinePoints(rectangle);
			foreach (var line in GridOutline)
				line.IsActive = true;
		}

		private static Rectangle CalculateNewTopLeft(Size sceneSize)
		{
			var xOffset = 0.5f;
			var yOffset = 0.5f;
			var aspect = sceneSize.Width / sceneSize.Height;
			float width;
			float height;
			if (aspect > 1)
			{
				yOffset = 1 / (2 * aspect);
				width = 1;
				height = width / aspect;
			}
			else if (aspect < 1)
			{
				xOffset = aspect / 2;
				height = 1;
				width = height * aspect;
			}
			else
			{
				height = 1;
				width = 1;
			}
			var topLeft = new Vector2D(0.5f - xOffset, 0.5f - yOffset);
			return new Rectangle(topLeft, new Size(width, height));
		}

		public void UpdateRenderlayerGrid()
		{
			foreach (var line in LinesInGridList)
				line.RenderLayer = GridRenderLayer;
		}

		public int GridRenderLayer { get; set; }
	}
}