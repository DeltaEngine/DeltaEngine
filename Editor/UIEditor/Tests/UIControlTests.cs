using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class UIControlTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			uiControl = new UIControl();
			uiEditorScene = new UIEditorScene(new MockService("", ""));
		}

		private UIControl uiControl;
		private UIEditorScene uiEditorScene;

		[Test]
		public void CannotSetTheControlSizeWhenWidthOrHeightIsLessThen10()
		{
			uiEditorScene.SelectedEntity2DList.Add(new Button(Rectangle.One));
			var material = new Material(new Size(8, 8), ShaderFlags.Colored);
			var oldControlSize = uiEditorScene.SelectedEntity2DList[0].DrawArea;
			uiControl.SetControlSize(uiEditorScene.SelectedEntity2DList[0], material, uiEditorScene);
			Assert.AreEqual(oldControlSize, uiEditorScene.SelectedEntity2DList[0].DrawArea);
		}

		[Test]
		public void SetTheControlSize()
		{
			uiEditorScene.SelectedEntity2DList.Add(new Button(Rectangle.One));
			var material = new Material(new Size(15, 20), ShaderFlags.Colored);
			uiEditorScene.SceneResolution = new Size(15, 20);
			var oldControlSize = uiEditorScene.SelectedEntity2DList[0].DrawArea;
			uiControl.SetControlSize(uiEditorScene.SelectedEntity2DList[0], material, uiEditorScene);
			Assert.AreNotEqual(oldControlSize, uiEditorScene.SelectedEntity2DList[0].DrawArea);
		}

		[Test]
		public void CannotChangeButtonWhenThereIsNoSelectedControl()
		{
			uiControl.ChangeToInteractiveButton(uiEditorScene);
			Assert.AreEqual(0, uiEditorScene.SelectedEntity2DList.Count);
		}

		[Test]
		public void ChangeMaterialsToDefaultMaterial()
		{
			var selectedControls = new List<Entity2D>();
			selectedControls.Add(new Slider(Rectangle.One));
			uiControl.SetMaterials(selectedControls);
			var defaultTheme = new Slider(Rectangle.One).Get<Theme>();
			Assert.AreEqual(defaultTheme.Slider.Name, uiControl.SelectedMaterial);
			Assert.AreEqual(defaultTheme.SliderPointerMouseover.Name, uiControl.SelectedHoveredMaterial);
			Assert.AreEqual(defaultTheme.SliderPointer.Name, uiControl.SelectedPressedMaterial);
			Assert.AreEqual(defaultTheme.SliderDisabled.Name, uiControl.SelectedDisabledMaterial);
		}
	}
}