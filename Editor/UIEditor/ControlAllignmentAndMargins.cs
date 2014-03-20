using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlAllignmentAndMargins
	{
		public ControlAllignmentAndMargins(UIEditorScene uiEditorScene)
		{
			this.uiEditorScene = uiEditorScene;
		}

		private readonly UIEditorScene uiEditorScene;

		public void ChangeBottomMargin(float value)
		{
			uiEditorScene.uiControl.BottomMargin = value;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (selectedEntity2D != null)
					selectedEntity2D.DrawArea = new Rectangle(selectedEntity2D.DrawArea.Left,
						value - selectedEntity2D.DrawArea.Height, selectedEntity2D.DrawArea.Width,
						selectedEntity2D.DrawArea.Height);
				uiEditorScene.uiControl.SetWidthAndHeight(selectedEntity2D.DrawArea);
			}
		}

		public void ChangeTopMargin(float value)
		{
			uiEditorScene.uiControl.TopMargin = value;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (selectedEntity2D != null)
					selectedEntity2D.DrawArea = new Rectangle(selectedEntity2D.DrawArea.Left, value,
						selectedEntity2D.DrawArea.Width, selectedEntity2D.DrawArea.Height);
				uiEditorScene.uiControl.SetWidthAndHeight(selectedEntity2D.DrawArea);
			}
		}

		public void ChangeLeftMargin(float value)
		{
			uiEditorScene.uiControl.LeftMargin = value;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (selectedEntity2D != null)
					selectedEntity2D.DrawArea = new Rectangle(value, selectedEntity2D.DrawArea.Top,
						selectedEntity2D.DrawArea.Width, selectedEntity2D.DrawArea.Height);
				uiEditorScene.uiControl.SetWidthAndHeight(selectedEntity2D.DrawArea);
			}
		}

		public void ChangeRightMargin(float value)
		{
			uiEditorScene.uiControl.RightMargin = value;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (selectedEntity2D != null)
					selectedEntity2D.DrawArea = new Rectangle(value - selectedEntity2D.DrawArea.Width,
						selectedEntity2D.DrawArea.Top, selectedEntity2D.DrawArea.Width,
						selectedEntity2D.DrawArea.Height);
				uiEditorScene.uiControl.SetWidthAndHeight(selectedEntity2D.DrawArea);
			}
		}

		public bool AllignControlsHorizontal(string value)
		{
			if (value == null || uiEditorScene.SelectedEntity2DList == null)
				return true;
			uiEditorScene.uiControl.HorizontalAllignment = value;
			if (value.Contains("Left"))
				foreach (var entity2D in uiEditorScene.SelectedEntity2DList)
					((Control)entity2D).LeftMargin = Left;
			if (value.Contains("Right"))
				foreach (var entity2D in uiEditorScene.SelectedEntity2DList)
					((Control)entity2D).RightMargin = Right;
			if (value.Contains("Center"))
				foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
					ChangeLeftMargin(0.5f - selectedEntity2D.DrawArea.Width / 2);
			return false;
		}

		private static readonly Margin Left = new Margin(Edge.Left, 0);
		private static readonly Margin Right = new Margin(Edge.Right, 0);

		public bool AllignControlVertical(string value)
		{
			if (value == null || uiEditorScene.SelectedEntity2DList == null)
				return true;
			uiEditorScene.uiControl.VerticalAllignment = value;
			if (value.Contains("Top"))
				foreach (var entity2D in uiEditorScene.SelectedEntity2DList)
					((Control)entity2D).TopMargin = Top;
			if (value.Contains("Bottom"))
				foreach (var entity2D in uiEditorScene.SelectedEntity2DList)
					((Control)entity2D).BottomMargin = Bottom;
			if (value.Contains("Center"))
				foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
					ChangeTopMargin(0.5f - selectedEntity2D.DrawArea.Height / 2);
			return false;
		}

		private static readonly Margin Top = new Margin(Edge.Top, 0);
		private static readonly Margin Bottom = new Margin(Edge.Bottom, 0);
	}
}