using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Multimedia;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.Core
{
	/// <summary>
	/// Viewport of the Editor, holds Screenspace, ensures that needed Commands are present and
	/// allows plugins to control the viewport up to a degree. Plugins that want to destroy all
	/// entities except for the viewport controls should call <see cref="DestroyRenderedEntities"/>
	/// </summary>
	public class EditorViewport
	{
		public EditorViewport(Window window)
		{
			Window = window;
			IsZoomingEnabled = true;
			ResetScreenSpace();
			Settings.Current.LimitFramerate = 60;
		}

		public Window Window { get; private set; }
		public bool IsZoomingEnabled { private get; set; }
		private Camera2DScreenSpace screenSpace;

		public void ResetScreenSpace()
		{
			ScreenSpace.Current = screenSpace = new Camera2DScreenSpace(Window);
		}

		public void OnViewportPanning(Vector2D start, Vector2D end, bool done)
		{
			screenSpace.LookAt += start - end;
		}

		public void OnViewPortZooming(float zoomDifference)
		{
			if (!IsZoomingEnabled)
				return;
			var changeByAmount = zoomDifference * 0.1f;
			if (screenSpace.Zoom + changeByAmount > 0.0f)
				screenSpace.Zoom += changeByAmount;
		}

		public void CenterViewOn(Vector2D centerPosition)
		{
			screenSpace.LookAt = centerPosition;
		}

		public void ZoomViewTo(float zoomAmount)
		{
			if (zoomAmount >= 0)
				screenSpace.Zoom = zoomAmount;
		}

		public void DestroyRenderedEntities()
		{
			var allEntities = EntitiesRunner.Current.GetAllEntities();
			foreach (var entity in allEntities)
				if (!entity.GetType().IsSubclassOf(typeof(SoundDevice)) &&
					entity.GetType() != typeof(Command) && !entity.GetType().IsSubclassOf(typeof(Trigger)))
					entity.IsActive = false;
			var temporaryCommands = EntitiesRunner.Current.GetEntitiesWithTag("temporary");
			for (int index = 0; index < temporaryCommands.Count; index++)
			{
				var command = temporaryCommands[index];
				command.IsActive = false;
			}
		}

		public void ResetViewportArea()
		{
			screenSpace.LookAt = Vector2D.Half;
			screenSpace.Zoom = 1.0f;
		}
	}
}