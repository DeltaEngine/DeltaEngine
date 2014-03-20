using DeltaEngine.Content;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlMaterialChanger 
	{
		public ControlMaterialChanger(UIEditorScene uiEditorScene)
		{
			this.uiEditorScene = uiEditorScene;
		}

		private readonly UIEditorScene uiEditorScene;

		public void ChangeMaterial(string newMaterialName)
		{
			foreach (Entity2D entity2D in uiEditorScene.SelectedEntity2DList)
			{
				var material = ContentLoader.Load<Material>(newMaterialName);
				if (entity2D == null)
					return;
				if (entity2D.GetType() == typeof(Button) || entity2D.GetType() == typeof(InteractiveButton))
					entity2D.Get<Theme>().Button = ContentLoader.Load<Material>(newMaterialName);
				else if (entity2D.GetType() == typeof(Slider))
				{
					entity2D.Get<Theme>().Slider = ContentLoader.Load<Material>(newMaterialName);
					entity2D.Get<Theme>().SliderDisabled = ContentLoader.Load<Material>(newMaterialName);
				}
				else if (entity2D.GetType() == typeof(Label))
				{
					entity2D.Set(ContentLoader.Load<Material>(newMaterialName));
					entity2D.Get<Theme>().Label = ContentLoader.Load<Material>(newMaterialName);
					entity2D.Set(ContentLoader.Load<Material>(newMaterialName).DefaultColor);
				}
				else
					entity2D.Set(material);
				uiEditorScene.uiControl.SetControlSize(entity2D, ContentLoader.Load<Material>(newMaterialName), uiEditorScene);
				var rect = entity2D.DrawArea;
				uiEditorScene.uiControl.EntityWidth = rect.Width;
				uiEditorScene.uiControl.EntityHeight = rect.Height;
			}
		}

		public void ChangeHoveredMaterial(string newMaterialName)
		{
			foreach (Entity2D entity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (entity2D == null)
					return;
				if (entity2D.GetType() == typeof(Button) || entity2D.GetType() == typeof(InteractiveButton))
				{
					var button = entity2D as Button;
					button.Get<Theme>().ButtonMouseover = ContentLoader.Load<Material>(newMaterialName);
				}
				else if (entity2D.GetType() == typeof(Slider))
				{
					var slider = entity2D as Slider;
					slider.Get<Theme>().SliderPointerMouseover = ContentLoader.Load<Material>(newMaterialName);
				}
			}
		}

		public void ChangePressedMaterial(string newMaterialName)
		{
			foreach (Entity2D entity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (entity2D == null)
					return;
				if (entity2D.GetType() == typeof(Button) || entity2D.GetType() == typeof(InteractiveButton))
				{
					var button = entity2D as Button;
					button.Get<Theme>().ButtonPressed = ContentLoader.Load<Material>(newMaterialName);
				}
				else if (entity2D.GetType() == typeof(Slider))
				{
					var slider = entity2D as Slider;
					slider.Get<Theme>().SliderPointer = ContentLoader.Load<Material>(newMaterialName);
				}
			}
		}

		public void ChangeDisabledMaterial(string newMaterialName)
		{
			foreach (Entity2D entity2D in uiEditorScene.SelectedEntity2DList)
			{
				if (entity2D == null)
					return;
				if (entity2D.GetType() == typeof(Button) || entity2D.GetType() == typeof(InteractiveButton))
				{
					var button = entity2D as Button;
					button.Get<Theme>().ButtonDisabled = ContentLoader.Load<Material>(newMaterialName);
				}
				else if (entity2D.GetType() == typeof(Slider))
				{
					var slider = entity2D as Slider;
					slider.Get<Theme>().SliderPointerDisabled = ContentLoader.Load<Material>(newMaterialName);
				}
			}
		}

		public static bool TryAddMaterial(string material)
		{
			try
			{
				var loadedMaterial = ContentLoader.Load<Material>(material);
				return !((ShaderWithFormat)loadedMaterial.Shader).Format.Is3D;
			}
			catch //ncrunch: no coverage start
			{
				return false;
			} //ncrunch: no coverage end
		}
	}
}