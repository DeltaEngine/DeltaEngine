using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlAdder
	{
		public ControlAdder()
		{
			Messenger.Default.Register<bool>(this, "SetDraggingImage", SetDraggingImage);
			Messenger.Default.Register<bool>(this, "SetDraggingButton", SetDraggingButton);
			Messenger.Default.Register<bool>(this, "SetDraggingLabel", SetDraggingLabel);
			Messenger.Default.Register<bool>(this, "SetDraggingSlider", SetDraggingSlider);
		}

		public void SetDraggingImage(bool draggingImage)
		{
			IsDraggingImage = draggingImage;
			IsDragging = draggingImage;
		}

		public bool IsDraggingImage { get; set; }
		public bool IsDragging { get; set; }

		public void SetDraggingButton(bool draggingButton)
		{
			IsDraggingButton = draggingButton;
			IsDragging = draggingButton;
		}

		public bool IsDraggingButton { get; set; }

		public void SetDraggingLabel(bool draggingLabel)
		{
			IsDraggingLabel = draggingLabel;
			IsDragging = draggingLabel;
		}

		public bool IsDraggingLabel { get; set; }

		public void SetDraggingSlider(bool draggingSlider)
		{
			IsDraggingSlider = draggingSlider;
			IsDragging = draggingSlider;
		}

		public bool IsDraggingSlider { get; set; }

		public void AddControl(Vector2D position, UIEditorScene uiEditorScene)
		{
			AddImage(position, uiEditorScene);
			AddButton(position, uiEditorScene);
			AddLabel(position, uiEditorScene);
			AddSlider(position, uiEditorScene);
			IsDragging = false;
		}

		private UIControl uiControl;

		public void AddImage(Vector2D position, UIEditorScene uiEditorScene)
		{
			uiControl = uiEditorScene.uiControl;
			if (!IsDraggingImage)
				return;
			var sprite = AddNewImageToList(position, uiEditorScene);
			uiEditorScene.SetSingleSelectedControl(sprite);
			uiControl.Index = uiEditorScene.Scene.Controls.IndexOf(sprite);
			uiControl.EntityWidth = sprite.DrawArea.Width;
			uiControl.EntityHeight = sprite.DrawArea.Height;
			uiControl.contentText = "";
			uiControl.ControlName = sprite.Name;
			IsDraggingImage = false;
			IsDragging = false;
			Messenger.Default.Send(sprite.Name, "AddToHierachyList");
		}

		public Picture AddNewImageToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newSprite = CreateANewDefaultImage(position);
			uiEditorScene.Scene.Add(newSprite);
			SetDefaultNamesOfNewImages(newSprite, uiEditorScene);
			return newSprite;
		}

		private static Picture CreateANewDefaultImage(Vector2D position)
		{
			var material = ContentExtensions.CreateDefaultMaterial2D();
			var newSprite = new Picture(new Theme(), material,
				Rectangle.FromCenter(position, new Size(0.05f)));
			newSprite.Set(BlendMode.Normal);
			return newSprite;
		}

		private static void SetDefaultNamesOfNewImages(Picture newSprite, UIEditorScene uiEditorScene)
		{
			uiEditorScene.UIImagesInList.Add(newSprite.Name);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = newSprite.Name;
		}
		
		public void AddButton(Vector2D position, UIEditorScene uiEditorScene)
		{
			uiControl = uiEditorScene.uiControl;
			if (!IsDraggingButton)
				return;
			var button = AddNewButtonToList(position, uiEditorScene);
			uiEditorScene.SetSingleSelectedControl(button);
			uiControl.Index = uiEditorScene.Scene.Controls.IndexOf(button);
			uiControl.contentText = "Default Button";
			uiControl.EntityWidth = button.DrawArea.Width;
			uiControl.EntityHeight = button.DrawArea.Height;
			uiControl.contentText = "Defualt button";
			uiControl.ControlName = button.Name;
			IsDraggingButton = false;
			IsDragging = false;
			Messenger.Default.Send(button.Name, "AddToHierachyList");
		}

		private static Button AddNewButtonToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newButton = new Button(new Theme(), Rectangle.FromCenter(position, new Size(0.2f, 0.1f)),
				"Default Button");
			uiEditorScene.Scene.Add(newButton);
			SetDefaultButtonName(newButton, uiEditorScene);
			return newButton;
		}

		private static void SetDefaultButtonName(Button newButton, UIEditorScene uiEditorScene)
		{
			uiEditorScene.UIImagesInList.Add(newButton.Name);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = newButton.Name;
		}

		public void AddLabel(Vector2D position, UIEditorScene uiEditorScene)
		{
			uiControl = uiEditorScene.uiControl;
			if (!IsDraggingLabel)
				return;
			var label = AddNewLabelToList(position, uiEditorScene);
			uiEditorScene.SetSingleSelectedControl(label);
			uiControl.Index = uiEditorScene.Scene.Controls.IndexOf(label);
			uiControl.contentText = "Default Label";
			uiControl.EntityWidth = label.DrawArea.Width;
			uiControl.EntityHeight = label.DrawArea.Height;
			uiControl.ControlName = label.Name;
			IsDraggingLabel = false;
			IsDragging = false;
			Messenger.Default.Send(label.Name, "AddToHierachyList");
		}

		private static Label AddNewLabelToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newLabel = new Label(new Theme(), Rectangle.FromCenter(position, new Size(0.2f, 0.1f)),
				"DefaultLabel");
			uiEditorScene.Scene.Add(newLabel);
			SetDefaultNameOfLable(newLabel, uiEditorScene);
			newLabel.Set(BlendMode.Normal);
			return newLabel;
		}

		private static void SetDefaultNameOfLable(Label newLabel, UIEditorScene uiEditorScene)
		{
			uiEditorScene.UIImagesInList.Add(newLabel.Name);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = newLabel.Name;
		}

		public void AddSlider(Vector2D position, UIEditorScene uiEditorScene)
		{
			uiControl = uiEditorScene.uiControl;
			if (!IsDraggingSlider)
				return;
			var slider = AddNewSliderToList(position, uiEditorScene);
			uiEditorScene.SetSingleSelectedControl(slider);
			uiControl.Index = uiEditorScene.Scene.Controls.IndexOf(slider);
			uiControl.contentText = "Default Slider";
			uiControl.EntityWidth = slider.DrawArea.Width;
			uiControl.EntityHeight = slider.DrawArea.Height;
			uiControl.ControlName = slider.Name;
			IsDraggingSlider = false;
			IsDragging = false;
			Messenger.Default.Send(slider.Name, "AddToHierachyList");
		}

		private static Slider AddNewSliderToList(Vector2D position, UIEditorScene uiEditorScene)
		{
			var newSlider = new Slider(new Theme(),
				Rectangle.FromCenter(position, new Size(0.15f, 0.03f)));
			uiEditorScene.Scene.Add(newSlider);
			SetDefaultSliderName(newSlider, uiEditorScene);
			return newSlider;
		}

		private static void SetDefaultSliderName(Slider newSlider, UIEditorScene uiEditorScene)
		{
			uiEditorScene.UIImagesInList.Add(newSlider.Name);
			if (uiEditorScene.UIImagesInList[0] == null)
				uiEditorScene.UIImagesInList[0] = newSlider.Name;
		}

		public void CreateCenteredControl(string newControl, UIEditorScene uiEditorScene)
		{
			uiControl = uiEditorScene.uiControl;
			IsDragging = true;
			if (newControl == "Image")
			{
				IsDraggingImage = true;
				AddImage(Vector2D.Half, uiEditorScene);
			}
			else if (newControl == "Button")
			{
				IsDraggingButton = true;
				AddButton(Vector2D.Half, uiEditorScene);
			}
			else if (newControl == "Label")
			{
				IsDraggingLabel = true;
				AddLabel(Vector2D.Half, uiEditorScene);
			}
			else if (newControl == "Slider")
			{
				IsDraggingSlider = true;
				AddSlider(Vector2D.Half, uiEditorScene);
			}
			uiEditorScene.uiControl.isClicking = false;
			if (uiEditorScene.SelectedEntity2DList.Count == 0)
				return;
			uiEditorScene.ControlProcessor.UpdateOutlines(uiEditorScene.SelectedEntity2DList);
		}

		public void AddControlToScene(Control control, Scene scene)
		{
			Control newControl = null;
			if (control.GetType() == typeof(Picture))
				newControl = new Picture((control as Picture).Theme, control.Material, control.DrawArea);
			else if (control.GetType() == typeof(Label))
			{
				newControl = new Label((control as Picture).Theme, control.DrawArea, (control as Label).Text);
				newControl.Set(control.Get<BlendMode>());
				newControl.Set(control.Material);
			}
			else if (control.GetType() == typeof(Button))
				newControl = new Button((control as Picture).Theme, control.DrawArea, (control as Button).Text);
			else if (control.GetType() == typeof(InteractiveButton))
				newControl = new InteractiveButton((control as Picture).Theme, control.DrawArea,
					(control as Button).Text);
			else if (control.GetType() == typeof(Slider))
				newControl = new Slider((control as Picture).Theme, control.DrawArea);
			newControl.Name = control.Name;
			if (newControl.Name == null && newControl.GetTags()[0] != null)
				newControl.Name = newControl.GetTags()[0];
			newControl.RenderLayer = control.RenderLayer;
			if (!control.Contains<AnchoringState>())
				newControl.Set(new AnchoringState()); //ncrunch: no coverage
			else
				newControl.Set(control.Get<AnchoringState>());
			scene.Add(newControl);
		}
	}
}