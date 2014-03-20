using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIControl
	{
		public float EntityWidth { get; set; }
		public float EntityHeight { get; set; }
		public int Index { get; set; }
		public bool isClicking;
		public string ControlName;
		public int controlLayer;
		public string contentText;
		public string VerticalAllignment;
		public string HorizontalAllignment;
		public float RightMargin;
		public float LeftMargin;
		public float TopMargin;
		public float BottomMargin;
		public string SelectedMaterial;
		public string SelectedHoveredMaterial;
		public string SelectedPressedMaterial;
		public string SelectedDisabledMaterial;
		public bool IsInteractiveButton;

		public void SetControlSize(Entity2D control, Material material, UIEditorScene scene)
		{
			if (material.DiffuseMap.PixelSize.Width < 10 || material.DiffuseMap.PixelSize.Height < 10)
				return;
			if (scene.SceneResolution.AspectRatio > 1)
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / scene.SceneResolution.Width)),
						((material.DiffuseMap.PixelSize.Height / scene.SceneResolution.Width))));
			else
				control.DrawArea = new Rectangle(control.DrawArea.TopLeft,
					new Size(((material.DiffuseMap.PixelSize.Width / scene.SceneResolution.Height)),
						((material.DiffuseMap.PixelSize.Height / scene.SceneResolution.Height))));
		}

		public void ChangeToInteractiveButton(UIEditorScene uiEditorScene)
		{
			if (uiEditorScene == null)
				return; //ncrunch: no coverage 
			for (int i = 0; i < uiEditorScene.SelectedEntity2DList.Count; i++)
			{
				var selectedEntity2D = uiEditorScene.SelectedEntity2DList[i];
				if (selectedEntity2D == null ||
					(selectedEntity2D.GetType() != typeof(Button) &&
						selectedEntity2D.GetType() != typeof(InteractiveButton)))
					return;
				var index = uiEditorScene.Scene.Controls.IndexOf(selectedEntity2D);
				var selectedIndex = uiEditorScene.SelectedEntity2DList.IndexOf(selectedEntity2D);
				uiEditorScene.Scene.Remove(selectedEntity2D);
				uiEditorScene.SelectedEntity2DList.RemoveAt(i);
				var renderLayer = selectedEntity2D.RenderLayer;
				var anchorState = selectedEntity2D.Get<AnchoringState>();
				var controlName = (selectedEntity2D as Control).Name;
				if (IsInteractiveButton)
					selectedEntity2D = new InteractiveButton(selectedEntity2D.Get<Theme>(),
						selectedEntity2D.DrawArea, ((Button)selectedEntity2D).Text);
				else
					selectedEntity2D = new Button(selectedEntity2D.Get<Theme>(), selectedEntity2D.DrawArea,
						((Button)selectedEntity2D).Text);
				selectedEntity2D.RenderLayer = renderLayer;
				selectedEntity2D.Set(anchorState);
				(selectedEntity2D as Control).Name = controlName;
				uiEditorScene.Scene.Controls.Insert(index, selectedEntity2D);
				uiEditorScene.SelectedEntity2DList.Insert(selectedIndex, selectedEntity2D);
			}
		}

		public void SetMaterials(List<Entity2D> selectedEntity2DList)
		{
			foreach (Entity2D selectedEntity2D in selectedEntity2DList)
				if (selectedEntity2D.GetType() == typeof(Button) ||
					selectedEntity2D.GetType() == typeof(InteractiveButton))
				{
					SelectedMaterial = selectedEntity2D.Get<Theme>().Button.Name;
					SelectedHoveredMaterial = selectedEntity2D.Get<Theme>().ButtonMouseover.Name;
					SelectedPressedMaterial = selectedEntity2D.Get<Theme>().ButtonPressed.Name;
					SelectedDisabledMaterial = selectedEntity2D.Get<Theme>().ButtonDisabled.Name;
				}
				else if (selectedEntity2D.GetType() == typeof(Slider))
				{
					SelectedMaterial = selectedEntity2D.Get<Theme>().Slider.Name;
					SelectedHoveredMaterial = selectedEntity2D.Get<Theme>().SliderPointerMouseover.Name;
					SelectedPressedMaterial = selectedEntity2D.Get<Theme>().SliderPointer.Name;
					SelectedDisabledMaterial = selectedEntity2D.Get<Theme>().SliderDisabled.Name;
				}
				else if (selectedEntity2D.GetType() == typeof(Label))
				{
					SelectedMaterial = selectedEntity2D.Get<Theme>().Label.Name;
					SelectedHoveredMaterial = null;
					SelectedPressedMaterial = null;
					SelectedDisabledMaterial = null;
				}
				else if (selectedEntity2D.Get<Material>() != null)
				{
					SelectedMaterial = selectedEntity2D.Get<Material>().ToString();
					SelectedHoveredMaterial = null;
					SelectedPressedMaterial = null;
					SelectedDisabledMaterial = null;
				}
			UpdateMaterialsInViewPort(selectedEntity2DList[selectedEntity2DList.Count - 1]);
		}

		public void UpdateMaterialsInViewPort(Entity2D selectedEntity2D)
		{
			Messenger.Default.Send(SelectedMaterial, "SetMaterial");
			Messenger.Default.Send(SelectedHoveredMaterial, "SetHoveredMaterial");
			Messenger.Default.Send(SelectedPressedMaterial, "SetPressedMaterial");
			Messenger.Default.Send(SelectedDisabledMaterial, "SetDisabledMaterial");
			SetEnabledButtons(selectedEntity2D != null ? selectedEntity2D.GetType().ToString() : "");
			Messenger.Default.Send("SetHorizontalAllignmentToNull", "SetHorizontalAllignmentToNull");
			Messenger.Default.Send("SetHorizontalAllignmentToNull", "SetHorizontalAllignmentToNull");
			Messenger.Default.Send("SetVerticalAllignmentToNull", "SetVerticalAllignmentToNull");
		}

		private static void SetEnabledButtons(string type)
		{
			Messenger.Default.Send(type, "EnabledHoveredButton");
			Messenger.Default.Send(type, "EnabledPressedButton");
			Messenger.Default.Send(type, "EnabledDisableButton");
			Messenger.Default.Send(type, "EnableButtonChanger");
			Messenger.Default.Send(type, "EnableTextChanger");
		}

		public void SetWidthAndHeight(Rectangle rect)
		{
			EntityWidth = rect.Width;
			EntityHeight = rect.Height;
			TopMargin = rect.Top;
			BottomMargin = rect.Bottom;
			LeftMargin = rect.Left;
			RightMargin = rect.Right;
		}

		public bool ChangeToInteractiveButton(bool value, UIEditorScene uiEditorScene)
		{
			if (IsInteractiveButton == value)
				return true;
			IsInteractiveButton = value;
			ChangeToInteractiveButton(uiEditorScene);
			return false;
		}

	}
}