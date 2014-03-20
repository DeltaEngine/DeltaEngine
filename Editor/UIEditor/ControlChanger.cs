using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlChanger
	{
		public void SetHeight(float value, UIEditorScene uiEditorScene)
		{
			var uiControl = uiEditorScene.uiControl;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (uiControl.isClicking || selectedEntity2D == null)
					return;
				uiControl.EntityHeight = value;
				var rect = selectedEntity2D.DrawArea;
				rect.Width = uiControl.EntityWidth;
				rect.Height = uiControl.EntityHeight;
				selectedEntity2D.DrawArea = rect;
				if (selectedEntity2D.GetType() == typeof(Button))
					ChangeButton((Button)selectedEntity2D, uiControl);
			}
			uiEditorScene.UpdateOutLine(uiEditorScene.SelectedEntity2DList);
		}

		public void SetWidth(float value, UIEditorScene uiEditorScene)
		{
			var uiControl = uiEditorScene.uiControl;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{	
				if (uiControl.isClicking || selectedEntity2D == null)
					return;
				uiControl.EntityWidth = value;
				var rect = selectedEntity2D.DrawArea;
				rect.Width = uiControl.EntityWidth;
				rect.Height = uiControl.EntityHeight;
				selectedEntity2D.DrawArea = rect;
				if (selectedEntity2D.GetType() == typeof(Button))
					ChangeButton((Button)selectedEntity2D, uiControl);
			}
			uiEditorScene.UpdateOutLine(uiEditorScene.SelectedEntity2DList);
		}

		private static void ChangeButton(Button button, UIControl uiControl)
		{
			button.Size = new Size(uiControl.EntityWidth, uiControl.EntityHeight);
			button.Text = uiControl.contentText;
		}

		public void SetContentText(string value, UIEditorScene uiEditorScene)
		{
			var uiControl = uiEditorScene.uiControl;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				uiControl.contentText = value;
				if (selectedEntity2D.GetType() == typeof(Button) ||
					selectedEntity2D.GetType() == typeof(InteractiveButton))
					ChangeButton((Button)selectedEntity2D, uiControl);
				if (selectedEntity2D.GetType() == typeof(Label))
					ChangeLabel((Label)selectedEntity2D, uiControl);
			}
		}

		private static void ChangeLabel(Label label, UIControl uiControl)
		{
			label.Size = new Size(uiControl.EntityWidth, uiControl.EntityHeight);
			label.Text = uiControl.contentText;
		}

		public void SetControlLayer(int value, UIEditorScene uiEditorScene)
		{
			var uiControl = uiEditorScene.uiControl;
			uiControl.controlLayer = value < 0 ? 0 : value;
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (selectedEntity2D == null)
					return;
				selectedEntity2D.RenderLayer = uiControl.controlLayer;
			}
		}

		public void SetSelectedControlNameInList(List<string> value, UIEditorScene uiEditorScene)
		{
			var uiControl = uiEditorScene.uiControl;
			if (value == null || uiControl.Index < 0 || uiEditorScene.Scene.Controls.Count <= 0)
				return;
			var entity = uiEditorScene.Scene.Controls[uiControl.Index];
			uiEditorScene.UpdateOutLine(uiEditorScene.SelectedEntity2DList);
			uiControl.ControlName = value[value.Count - 1];
			uiControl.controlLayer = entity.RenderLayer;
			uiControl.EntityWidth = entity.DrawArea.Width;
			uiControl.EntityHeight = entity.DrawArea.Height;
			if (uiEditorScene.SelectedEntity2DList.GetType() == typeof(Button) ||
				uiEditorScene.SelectedEntity2DList.GetType() == typeof(InteractiveButton))
				uiControl.contentText = ((Button)entity).Text;
			else if (uiEditorScene.SelectedEntity2DList.GetType() == typeof(Label))
				uiControl.contentText = ((Label)entity).Text;
			else
				uiControl.contentText = "";
		}

		public void ChangeControlName(string controlName, UIEditorScene uiEditorScene)
		{
			foreach (var selectedEntity2D in uiEditorScene.SelectedEntity2DList)
			{
				uiEditorScene.uiControl.ControlName = controlName;
				Messenger.Default.Send(controlName, "ChangeSelectedControlName");
				var spriteListIndex = uiEditorScene.Scene.Controls.IndexOf(selectedEntity2D);
				if (spriteListIndex < 0)
					return; //ncrunch: no coverage 
				uiEditorScene.UIImagesInList[spriteListIndex] = controlName;
				for (int index = 0; index < uiEditorScene.SelectedControlNamesInList.Count; index++)
					uiEditorScene.SelectedControlNamesInList[index] = controlName;
				(selectedEntity2D as Control).Name = controlName; 
			}
		}

		public void ChangeUIControlWidthAndHeight(Control control, UIControl uiControl)
		{
			Rectangle drawArea;
			if (control.GetType() == typeof(Button))
			{
				drawArea = new Rectangle(control.DrawArea.TopLeft, (control).Size);
				uiControl.IsInteractiveButton = false;
			}
			else if (control.GetType() == typeof(InteractiveButton))
			{
				drawArea = Rectangle.FromCenter(control.Center, control.Size);
				if (uiControl.IsInteractiveButton == false)
					uiControl.IsInteractiveButton = true;
			}
			else
				drawArea = control.DrawArea;
			uiControl.SetWidthAndHeight(drawArea);
		}
	}
}