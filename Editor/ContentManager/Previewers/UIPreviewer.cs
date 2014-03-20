using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class UIPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var scene = ContentLoader.Load<Scene>(contentName);
			Scene = new Scene();
			foreach (var control in scene.Controls)
			{
				control.IsActive = true;
				if (!control.Contains<AnchoringState>())
					control.Add(new AnchoringState());
				CheckIfAnyMaterialIsCorrupt((Control)control);
				AddControlToScene((Control)control);
				control.IsActive = false;
			}
		}

		private Scene Scene;

		public void AddControlToScene(Control control)
		{
			Control newControl = null;
			if (control.GetType() == typeof(Picture))
				newControl = new Picture(control.Get<Theme>(), control.Get<Material>(), control.DrawArea);
			else if (control.GetType() == typeof(Label))
			{
				newControl = new Label(control.Get<Theme>(), control.DrawArea, (control as Label).Text);
				newControl.Set(control.Get<BlendMode>());
				newControl.Set(control.Get<Material>());
			}
			else if (control.GetType() == typeof(Button))
				newControl = new Button(control.Get<Theme>(), control.DrawArea, (control as Button).Text);
			else if (control.GetType() == typeof(InteractiveButton))
				newControl = new InteractiveButton(control.Get<Theme>(), control.DrawArea,
					(control as Button).Text);
			else if (control.GetType() == typeof(Slider))
				newControl = new Slider(control.Get<Theme>(), control.DrawArea);
			newControl.RenderLayer = control.RenderLayer;
			if (!newControl.Contains<AnchoringState>())
				newControl.Add(new AnchoringState()); //ncrunch: no coverage
			CheckIfAnyMaterialIsCorrupt(newControl);
			Scene.Add(newControl);
		}

		private static void CheckIfAnyMaterialIsCorrupt(Control newControl)
		{
			if (newControl.Get<Material>().Shader == null)
				newControl.Set(new Theme().Button);
			if (newControl.GetType() == typeof(Button) ||
				newControl.GetType() == typeof(InteractiveButton))
				ChangeCorruptedButtonMaterial(newControl);
			if (newControl.GetType() == typeof(Slider))
				ChangeCorruptedSliderMaterial(newControl);
			if (newControl.GetType() == typeof(Label))
				if (newControl.Get<Theme>().Label.Shader == null)
					newControl.Get<Theme>().Label = new Theme().Label;
		}

		private static void ChangeCorruptedButtonMaterial(Control newControl)
		{
			var theme = newControl.Get<Theme>();
			if (theme.Button.Shader == null)
				theme.Button = new Theme().Button;
			if (theme.ButtonDisabled.Shader == null)
				theme.ButtonDisabled = new Theme().ButtonDisabled;
			if (theme.ButtonMouseover.Shader == null)
				theme.ButtonMouseover = new Theme().ButtonMouseover;
			if (theme.ButtonPressed.Shader == null)
				theme.ButtonPressed = new Theme().ButtonPressed;
		}

		private static void ChangeCorruptedSliderMaterial(Control newControl)
		{
			var theme = newControl.Get<Theme>();
			if (theme.Slider.Shader == null)
				theme.Slider = new Theme().Slider;
			if (theme.SliderDisabled.Shader == null)
				theme.SliderDisabled = new Theme().SliderDisabled;
			if (theme.SliderPointerMouseover.Shader == null)
				theme.SliderPointerMouseover = new Theme().SliderPointerMouseover;
			if (theme.SliderPointer.Shader == null)
				theme.SliderPointer = new Theme().SliderPointer;
			if (theme.SliderPointerDisabled.Shader == null)
				theme.SliderPointerDisabled = new Theme().SliderPointerDisabled;
		}
	}
}