using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlProcessor
	{
		public ControlProcessor()
		{
			Outlines = new List<Line2D[]>();
			GizmoList = new Sprite[2];
			GeneralOutline = new Line2D[4];
		}

		public List<Line2D[]> Outlines { get; private set; }
		public Line2D[] GeneralOutline { get; private set; }
		public Sprite[] GizmoList { get; private set; }

		private static readonly Color SelectionColor = Color.White; //ncrunch: no coverage

		internal void UpdateOutlines(List<Entity2D> selectedControlList)
		{
			ClearLines();
			foreach (Entity2D selectedControl in selectedControlList)
			{
				var newOutlineColor = new Line2D[4];
				CreateOutlinesAndGizmos(newOutlineColor);
				if (selectedControl == null)
					return; //ncrunch: no coverage
				var rectangleCorners = GetRotatedRectangelCorners(selectedControl.DrawArea,
					selectedControl.Rotation);
				newOutlineColor[0].StartPoint = rectangleCorners[0];
				newOutlineColor[0].EndPoint = rectangleCorners[1];
				newOutlineColor[1].StartPoint = rectangleCorners[0];
				newOutlineColor[1].EndPoint = rectangleCorners[2];
				newOutlineColor[2].StartPoint = rectangleCorners[2];
				newOutlineColor[2].EndPoint = rectangleCorners[3];
				newOutlineColor[3].StartPoint = rectangleCorners[3];
				newOutlineColor[3].EndPoint = rectangleCorners[1];
				Outlines.Add(newOutlineColor);
			}
			CreateGeneralOutline(selectedControlList);
		}

		private void ClearLines()
		{
			foreach (Line2D line2D in Outlines.SelectMany(lines => lines))
				line2D.IsActive = false;
			foreach (Line2D line2D in GeneralOutline.Where(line2D => line2D != null))
				line2D.IsActive = false;
			foreach (Sprite gizmo in GizmoList.Where(gizmo => gizmo != null))
				gizmo.IsActive = false;
		}

		private static void CreateOutlinesAndGizmos(Line2D[] newLines)
		{
			newLines[0] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			newLines[1] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			newLines[2] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
			newLines[3] = new Line2D(Vector2D.Unused, Vector2D.Unused, SelectionColor)
			{
				RenderLayer = 1000
			};
		}

		private static Vector2D[] GetRotatedRectangelCorners(Rectangle rectangle, float angle)
		{
			var newCorners = new Vector2D[4];
			newCorners[0] = rectangle.TopLeft.RotateAround(rectangle.Center, angle);
			newCorners[1] = rectangle.TopRight.RotateAround(rectangle.Center, angle);
			newCorners[2] = rectangle.BottomLeft.RotateAround(rectangle.Center, angle);
			newCorners[3] = rectangle.BottomRight.RotateAround(rectangle.Center, angle);
			return newCorners;
		}

		private void CreateGeneralOutline(List<Entity2D> selectedControlList)
		{
			var boudingBox = CalculateBoundingBox(selectedControlList);
			GeneralOutline[0] = new Line2D(boudingBox.TopLeft, boudingBox.TopRight, SelectionColor);
			GeneralOutline[1] = new Line2D(boudingBox.TopLeft, boudingBox.BottomLeft, SelectionColor);
			GeneralOutline[2] = new Line2D(boudingBox.BottomLeft, boudingBox.BottomRight, SelectionColor);
			GeneralOutline[3] = new Line2D(boudingBox.TopRight, boudingBox.BottomRight, SelectionColor);
			Material material = ContentExtensions.CreateDefaultMaterial2D(Color.Blue);
			GizmoList[0] = new Sprite(material,
				new Rectangle(new Vector2D(boudingBox.TopRight.X - 0.02f, boudingBox.TopRight.Y),
					new Size(0.02f, 0.02f)));
			GizmoList[1] = new Sprite(material,
				new Rectangle(
					new Vector2D(boudingBox.BottomRight.X - 0.02f, boudingBox.BottomRight.Y - 0.02f),
					new Size(0.02f, 0.02f)));
		}

		public Rectangle CalculateBoundingBox(List<Entity2D> selectedControlList)
		{
			if (selectedControlList.Count == 0)
				return Rectangle.Zero;
			var rect =
				selectedControlList[0].DrawArea.GetBoundingBoxAfterRotation(selectedControlList[0].Rotation);
			for (int i = 1; i < selectedControlList.Count; i++)
				rect =
					rect.Merge(
						selectedControlList[i].DrawArea.GetBoundingBoxAfterRotation(
							selectedControlList[i].Rotation));
			return rect;
		}

		internal void MoveImage(Vector2D mousePosition, List<Entity2D> selectedEntity2DList,
			bool isDragging, UIEditorScene scene)
		{
			if (selectedEntity2DList.Count == 0 || isDragging)
				return; //ncrunch: no coverage 
			if (scene.UISceneGrid.GridWidth == 0 || scene.UISceneGrid.GridHeight == 0 ||
				!scene.IsSnappingToGrid || !scene.IsDrawingGrid)
				MoveImageWithoutGrid(mousePosition, selectedEntity2DList);
			else
				MoveImageUsingTheGrid(mousePosition, selectedEntity2DList, scene);
			UpdateOutlines(selectedEntity2DList);
		}

		private void MoveImageWithoutGrid(Vector2D mousePosition, List<Entity2D> selectedEntity2DList)
		{
			var relativePosition = mousePosition - lastMousePosition;
			lastMousePosition = mousePosition;
			foreach (var control in selectedEntity2DList)
				control.Center += relativePosition;
		}

		internal Vector2D lastMousePosition = Vector2D.Unused;

		private void MoveImageUsingTheGrid(Vector2D mousePosition,
			List<Entity2D> selectedEntity2DList, UIEditorScene scene)
		{
			Vector2D topLeft = scene.UISceneGrid.LinesInGridList[0].TopLeft;
			var relativePosition = mousePosition - lastMousePosition;
			foreach (var selectedEntity2D in selectedEntity2DList)
			{
				float posX = (selectedEntity2D.DrawArea.Left + relativePosition.X);
				float posY = (selectedEntity2D.DrawArea.Top + relativePosition.Y);
				var sceneSize =
					ScreenSpace.Current.FromPixelSpace(new Size(scene.SceneResolution.Width,
						scene.SceneResolution.Height));
				var tileSize =
					ScreenSpace.Current.FromPixelSpace(new Size(scene.UISceneGrid.GridWidth,
						scene.UISceneGrid.GridHeight));
				float tilewidth;
				float tileheight;
				if (sceneSize.Width > sceneSize.Height)
				{
					tilewidth = 1 / (sceneSize.Width / tileSize.Width);
					tileheight = 1 / (sceneSize.Width / tileSize.Height);
				}
				else
				{
					tilewidth = 1 / (sceneSize.Height / tileSize.Width);
					tileheight = 1 / (sceneSize.Height / tileSize.Height);
				}
				var columnNumberInGrid = (int)Math.Round((posX - topLeft.X) / tilewidth);
				var rowNumberInGrid = (int)Math.Round((posY - topLeft.Y) / tileheight);
				selectedEntity2D.DrawArea = new Rectangle((tilewidth * columnNumberInGrid) + topLeft.X,
					(tileheight * rowNumberInGrid) + topLeft.Y, selectedEntity2D.DrawArea.Width,
					selectedEntity2D.DrawArea.Height);
				if (spritePos == selectedEntity2D.Center)
					return; //ncrunch: no coverage
				lastMousePosition = mousePosition;
				spritePos = selectedEntity2D.Center;
			}
		}

		private Vector2D spritePos;
	}
}