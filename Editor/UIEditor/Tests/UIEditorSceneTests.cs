using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class UIEditorSceneTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			uiEditorScene = new UIEditorScene(new MockService("", ""));
			uiEditorScene.ControlProcessor = new ControlProcessor();
			uiEditorScene.SelectedEntity2DList.Add(new Button(new Rectangle(0, 0, 1, 1)));
		}

		private UIEditorScene uiEditorScene;

		[Test, CloseAfterFirstFrame]
		public void ChangeHoveredMaterial()
		{
			uiEditorScene.controlMaterialChanger.ChangeHoveredMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D",
				uiEditorScene.SelectedEntity2DList[0].Get<Theme>().ButtonMouseover.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePressedMaterial()
		{
			uiEditorScene.controlMaterialChanger.ChangePressedMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D",
				uiEditorScene.SelectedEntity2DList[0].Get<Theme>().ButtonPressed.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeDisabledMaterial()
		{
			uiEditorScene.controlMaterialChanger.ChangeDisabledMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D",
				uiEditorScene.SelectedEntity2DList[0].Get<Theme>().ButtonDisabled.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeOutlineWithBiggerHeight()
		{
			uiEditorScene.SceneResolution = new Size(500, 300);
			uiEditorScene.UISceneGrid.UpdateGridOutline(uiEditorScene.SceneResolution);
			Assert.AreEqual(0.2f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].Y, 0.01f);
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].X, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeOutlineWithWidthEqualToHeight()
		{
			uiEditorScene.SceneResolution = new Size(500);
			uiEditorScene.UISceneGrid.UpdateGridOutline(uiEditorScene.SceneResolution);
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].X, 0.01f);
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].Y, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotDrawOutlineWhenWidthOrHeightIsZero()
		{
			uiEditorScene.SceneResolution = new Size(500, 300);
			uiEditorScene.UISceneGrid.UpdateGridOutline(uiEditorScene.SceneResolution);
			Assert.AreEqual(0.2f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].Y, 0.01f);
			uiEditorScene.SceneResolution = new Size(0, 300);
			uiEditorScene.UISceneGrid.UpdateGridOutline(uiEditorScene.SceneResolution);
			Assert.AreEqual(0.2f, uiEditorScene.UISceneGrid.GridOutline[0].Points[0].Y, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingGridWithBiggerHeight()
		{
			uiEditorScene.SceneResolution = new Size(500, 300);
			uiEditorScene.ChangeGrid("300 x 500");
			Assert.AreEqual(0.2f, uiEditorScene.UISceneGrid.LinesInGridList[0].Points[0].Y, 0.01f);
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.LinesInGridList[0].Points[0].X, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void DrawinGridWithWidthEqualToHeight()
		{
			uiEditorScene.SceneResolution = new Size(500);
			uiEditorScene.ChangeGrid("500 x 500");
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.LinesInGridList[0].Points[0].X, 0.01f);
			Assert.AreEqual(0.0f, uiEditorScene.UISceneGrid.LinesInGridList[0].Points[0].Y, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void CanNotDrawGridWhenWidthOrHeightIs0()
		{
			uiEditorScene.SceneResolution = new Size(500, 300);
			uiEditorScene.ChangeGrid("300 x 500");
			Assert.AreEqual(0.2f, uiEditorScene.UISceneGrid.LinesInGridList[0].Points[0].Y, 0.01f);
			uiEditorScene.SceneResolution = new Size(0, 300);
			uiEditorScene.UISceneGrid.DrawGrid();
			uiEditorScene.ChangeGrid("0 x 500");
			Assert.AreEqual(0, uiEditorScene.UISceneGrid.LinesInGridList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAllMaterialsForAButton()
		{
			var theme = GetCreateNewControlAndChangeMaterials(new Button(Rectangle.One));
			Assert.AreEqual("DeltaEngineLogo", theme.Button.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.ButtonMouseover.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.ButtonPressed.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.ButtonDisabled.DiffuseMap.Name);
		}

		private Theme GetCreateNewControlAndChangeMaterials(Control control)
		{
			uiEditorScene.SelectedEntity2DList.Clear();
			uiEditorScene.SelectedEntity2DList.Add(control);
			ChangeAllMaterials();
			var theme = uiEditorScene.SelectedEntity2DList[0].Get<Theme>();
			return theme;
		}

		private void ChangeAllMaterials()
		{
			uiEditorScene.controlMaterialChanger.ChangeMaterial("deltalogo");
			uiEditorScene.controlMaterialChanger.ChangeHoveredMaterial("deltalogo");
			uiEditorScene.controlMaterialChanger.ChangePressedMaterial("deltalogo");
			uiEditorScene.controlMaterialChanger.ChangeDisabledMaterial("deltalogo");
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAllMaterialsForASlider()
		{
			var theme = GetCreateNewControlAndChangeMaterials(new Slider(Rectangle.One));
			Assert.AreEqual("DeltaEngineLogo", theme.Slider.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.SliderPointer.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.SliderPointerMouseover.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", theme.SliderDisabled.DiffuseMap.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAllMaterialsForALabel()
		{
			var theme = GetCreateNewControlAndChangeMaterials(new Label(Rectangle.One));
			Assert.AreEqual("DeltaEngineLogo", theme.Label.DiffuseMap.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotChangeMaterialsWhenNothingIsSelected()
		{
			uiEditorScene.SelectedEntity2DList.Clear();
			ChangeAllMaterials();
			Assert.AreEqual(0, uiEditorScene.SelectedEntity2DList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void DeletingAControlWillSelectTheNextOne()
		{
			CreateNewButtons();
			uiEditorScene.SelectedEntity2DList.Clear();
			uiEditorScene.SelectedEntity2DList.Add(uiEditorScene.Scene.Controls[2]);
			Assert.AreEqual("2", ((Button)uiEditorScene.SelectedEntity2DList[0]).Text);
			uiEditorScene.CanDeleteSelectedControl = true;
			uiEditorScene.DeleteSelectedControl("");
			Assert.AreEqual("3", ((Button)uiEditorScene.SelectedEntity2DList[0]).Text);
		}

		private void CreateNewButtons()
		{
			for (int i = 0; i < 5; i++)
			{
				var button = new Button(Rectangle.One, i.ToString());
				button.AddTag(i.ToString());
				button.RenderLayer = i;
				uiEditorScene.Scene.Add(button);
				uiEditorScene.UIImagesInList.Add(i.ToString());
			}
		}

		[Test, CloseAfterFirstFrame]
		public void SelectAnInterActiveButton()
		{
			var button = new InteractiveButton(Rectangle.One);
			button.AddTag("New Button");
			uiEditorScene.Scene.Add(button);
			uiEditorScene.SetEntity2D(button);
			Assert.AreEqual(button, uiEditorScene.SelectedEntity2DList[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectControlWithHighestRenderlayerWhenSelectingNewControl()
		{
			CreateNewButtons();
			var button = new InteractiveButton(Rectangle.One);
			button.AddTag("New Button");
			button.RenderLayer = 6;
			uiEditorScene.Scene.Controls.Insert(1, button);
			uiEditorScene.FindEntity2DOnPosition(new Vector2D(0.5f, 0.5f));
			Assert.AreEqual(6, uiEditorScene.SelectedEntity2DList[0].RenderLayer);
		}

		[Test, CloseAfterFirstFrame, RequiresSTA]
		public void DeleteUIElement()
		{
			AddNewSprite();
			AddNewSprite();
			uiEditorScene.uiControl.Index = 1;
			var keyboard = Resolve<MockKeyboard>();
			keyboard.SetKeyboardState(Key.Delete, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			uiEditorScene.CanDeleteSelectedControl = true;
			uiEditorScene.DeleteSelectedControl("");
			Assert.AreEqual(1, uiEditorScene.Scene.Controls.Count);
		}

		private void AddNewSprite()
		{
			uiEditorScene.controlAdder.SetDraggingImage(true);
			uiEditorScene.controlAdder.AddImage(new Vector2D(0.5f, 0.5f), uiEditorScene);
			uiEditorScene.SelectedControlNamesInList.Add("NewSprite0");
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageWithoutGrid()
		{
			AddNewSprite();
			uiEditorScene.controlTransformer.uiMouseCursor.CurrentCursor = ControlTransformer.MoveCursor;
			var startPosition = uiEditorScene.SelectedEntity2DList[0].DrawArea.TopLeft;
			var mouse = Resolve<MockMouse>();
			mouse.SetNativePosition(new Vector2D(0.55f, 0.55f));
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(startPosition, uiEditorScene.SelectedEntity2DList[0].DrawArea.TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageUsingGrid()
		{
			AddNewSprite();
			AddNewSprite();
			var startPosition = uiEditorScene.Scene.Controls[0].TopLeft;
			uiEditorScene.IsSnappingToGrid = true;
			uiEditorScene.NewGridWidth = 1;
			uiEditorScene.NewGridHeight = 1;
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			Assert.AreEqual(startPosition, uiEditorScene.Scene.Controls[1].TopLeft);
			Assert.IsTrue(uiEditorScene.IsSnappingToGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageUsingGridWithNewResolution()
		{
			AddNewSprite();
			AddNewSprite();
			var startPosition = uiEditorScene.Scene.Controls[0].TopLeft;
			uiEditorScene.IsSnappingToGrid = true;
			uiEditorScene.SceneResolution = new Size(5, 10);
			var mouse = Resolve<MockMouse>();
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			Assert.AreEqual(startPosition, uiEditorScene.Scene.Controls[1].TopLeft);
			Assert.IsTrue(uiEditorScene.IsSnappingToGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaterial()
		{
			uiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			AddNewButton();
			uiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			uiEditorScene.SelectedEntity2DList.Clear();
			uiEditorScene.SelectedEntity2DList.Add(new Sprite(ContentLoader.Load<Material>("material"),
				Rectangle.One));
			uiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D", uiEditorScene.SelectedEntity2DList[0].Get<Material>().Name);
		}

		private void AddNewButton()
		{
			uiEditorScene.controlAdder.SetDraggingButton(true);
			uiEditorScene.controlAdder.AddButton(new Vector2D(0.5f, 0.5f), uiEditorScene);
			uiEditorScene.SelectedControlNamesInList.Add("NewButton0");
		}
	}
}