using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D.Shapes;

namespace DeltaEngine.Editor.LevelEditor
{
	public class LevelEditorCommands
	{
		public LevelEditorCommands(LevelEditorViewModel viewModel)
		{
			this.viewModel = viewModel;
			Level = viewModel.Level;
			Renderer = viewModel.renderer;
			levelObjectHandler = viewModel.levelObjectHandler;
			SelectedLevelObject = viewModel.SelectedLevelObject;
			SelectedTileType = viewModel.SelectedTileType;
			cameraSliders = viewModel.cameraSliders;
		}

		private readonly LevelEditorViewModel viewModel;
		public Level Level { get; set; }
		public LevelDebugRenderer Renderer { private get; set; }
		private readonly LevelObjectHandler levelObjectHandler;
		public string SelectedLevelObject { private get; set; }
		public LevelTileType SelectedTileType { private get; set; }
		private readonly CameraSliders cameraSliders;

		public void SetCommands()
		{
			var leftClickTrigger = new MouseButtonTrigger();
			leftClickTrigger.AddTag("temporary");
			var leftClickCommand = new Command(LeftMouseButton).Add(leftClickTrigger);
			leftClickCommand.AddTag("temporary");
			var middleDragTrigger = new MouseDragTrigger(MouseButton.Middle);
			middleDragTrigger.AddTag("temporary");
			var middleDragCommand = new Command(MiddleMouseDrag).Add(middleDragTrigger);
			middleDragCommand.AddTag("temporary");
			var dragLeftTrigger = new MouseDragTrigger();
			dragLeftTrigger.AddTag("temporary");
			var dragLeftCommand = new Command(LeftMouseDrag).Add(dragLeftTrigger);
			dragLeftCommand.AddTag("temporary");
			var zoomTrigger = new MouseZoomTrigger();
			zoomTrigger.AddTag("temporary");
			var zoomCommand = new Command(Zoom).Add(zoomTrigger);
			zoomCommand.AddTag("temporary");
			var leftReleaseTrigger = new MouseButtonTrigger(State.Releasing);
			leftReleaseTrigger.AddTag("temporary");
			var leftReleaseCommand = new Command(LeftMouseRelease).Add(leftReleaseTrigger);
			leftReleaseCommand.AddTag("temporary");
		}

		public void LeftMouseButton(Vector2D screenPosition)
		{
			int levelObjectIndex;
			if (viewModel.Is3D)
			{
				var position = GetPosition(screenPosition);
				if (position == null || !IsPositionInGrid((Vector2D)position))
					return;
				levelObjectIndex = GetLevelObjectIndex((Vector2D)position);
				PlaceLevelObject(levelObjectIndex, (Vector2D)position);
			}
			else
			{
				var position = Level.GetTileIndex(screenPosition);
				if (!Level.IsInsideLevelGrid(position))
					return;
				levelObjectIndex = GetLevelObjectIndex(position);
				PlaceLevelObject(levelObjectIndex, Renderer.GetRectangleDrawPosition(position));
			}
			StartDragIndex = levelObjectIndex;
		}

		public int StartDragIndex { get; private set; }

		private static Vector2D? GetPosition(Vector2D screenPosition)
		{
			var position = Level.GetIntersectionWithFloor(screenPosition);
			if (position != null)
				return new Vector2D((float)(Math.Floor(position.Value.X) + Size.Half.Width),
					(float)(Math.Floor(position.Value.Y) + Size.Half.Height));
			return null;
		}

		private bool IsPositionInGrid(Vector2D position)
		{
			return !(position.X < -Level.Size.Width * 0.5f) && !(position.X > Level.Size.Width * 0.5f) &&
				!(position.Y < -Level.Size.Height * 0.5f) && !(position.Y > Level.Size.Height * 0.5f);
		}

		private int GetLevelObjectIndex(Vector2D position)
		{
			if (viewModel.Is3D)
				return (int)(position.X + Level.Size.Width * 0.5f) +
					(int)(position.Y + Level.Size.Height * 0.5f) * (int)Level.Size.Width;
			return (int)position.X + (int)position.Y * (int)Level.Size.Width;
		}

		private void PlaceLevelObject(int levelObjectIndex, Vector2D position)
		{
			if (levelObjectHandler.ObjectList[levelObjectIndex] == null && SelectedLevelObject != null)
				levelObjectHandler.PlaceLevelObject(position, SelectedLevelObject, levelObjectIndex);
		}

		private void RotateLevelObject(int levelObjectIndex)
		{
			if (levelObjectHandler.ObjectList[levelObjectIndex] != null)
				levelObjectHandler.RotateObject(levelObjectIndex);
		}

		private void SetTileWithScreenPosition(Vector2D screenPosition)
		{
			if (SelectedTileType != LevelTileType.NoSelection)
				Level.SetTileWithScreenPosition(screenPosition, SelectedTileType);
		}

		public void LeftMouseDrag(Vector2D screenPosition)
		{
			SetTileWithScreenPosition(screenPosition);
			if (viewModel.Is3D)
			{
				var position = GetPosition(screenPosition);
				if (position == null || !IsPositionInGrid((Vector2D)position))
					return;
				RotateLevelObject(GetLevelObjectIndex((Vector2D)position));
				Renderer.UpdateTileAt((Vector2D)position, GetLevelObjectIndex((Vector2D)position));
			}
			else
			{
				var position = Level.GetTileIndex(screenPosition);
				if (!Level.IsInsideLevelGrid(position))
					return;
				RotateLevelObject(GetLevelObjectIndex(position));
				Renderer.UpdateTile(position);
			}
		}

		public void LeftMouseRelease(Vector2D screenPosition)
		{
			if (viewModel.Is3D)
			{
				var position = GetPosition(screenPosition);
				if (position == null || !IsPositionInGrid((Vector2D)position))
					return;
				MoveLevelObject((Vector2D)position, GetLevelObjectIndex((Vector2D)position));
			}
			else
			{
				var position = Level.GetTileIndex(screenPosition);
				if (!Level.IsInsideLevelGrid(position))
					return;
				MoveLevelObject(Renderer.GetRectangleDrawPosition(position), GetLevelObjectIndex(position));
			}
		}

		private void MoveLevelObject(Vector2D position, int levelObjectIndex)
		{
			if (StartDragIndex < 0 || levelObjectHandler.ObjectList[StartDragIndex] == null)
				return;
			if (levelObjectHandler.ObjectList[levelObjectIndex] == null)
				levelObjectHandler.MoveObject(position, StartDragIndex, levelObjectIndex);
		}

		public void SetTileToNothingAndRemoveLevelObject(Vector2D screenPosition)
		{
			Level.SetTileWithScreenPosition(screenPosition, LevelTileType.Nothing);
			int levelObjectIndex;
			if (viewModel.Is3D)
			{
				var position = GetPosition(screenPosition);
				if (position == null || !IsPositionInGrid((Vector2D)position))
					return;
				levelObjectIndex = GetLevelObjectIndex((Vector2D)position);
			}
			else
			{
				var position = Level.GetTileIndex(screenPosition);
				if (!Level.IsInsideLevelGrid(position))
					return;
				levelObjectIndex = GetLevelObjectIndex(position);
			}
			Renderer.RecreateTiles();
			if (levelObjectHandler.ObjectList[levelObjectIndex] == null)
				return;
			levelObjectHandler.ObjectList[levelObjectIndex].Dispose();
			levelObjectHandler.ObjectList[levelObjectIndex] = null;
		}

		//ncrunch: no coverage start
		public void Zoom()
		{
			if (!viewModel.Is3D)
				cameraSliders.CalculateSliderPositionAndScale();
		}

		public void MiddleMouseDrag()
		{
			Renderer.RecreateTiles();
			cameraSliders.CalculateSliderPositionAndScale();
		}
	}
}