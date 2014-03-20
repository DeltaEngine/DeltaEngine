using System.Collections.Generic;
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
	public class UIEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			mockService = new MockService("TestUser", "NewProjectWithoutContent");
			viewModel = new UIEditorViewModel(mockService);
			viewModel.ClearScene("");
			viewModel.UiEditorScene.RefreshOnContentChange();
			viewModel.Reset();
		}

		private UIEditorViewModel viewModel;
		private MockService mockService;

		[Test, CloseAfterFirstFrame]
		public void CannotAddIfImageDraggingNotSet()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingImage(false);
			viewModel.UiEditorScene.controlAdder.AddImage(new Vector2D(1.0f, 1.0f),
				viewModel.UiEditorScene);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(2, viewModel.ContentImageListList.Count);
		}

		[Test]
		public void UIShouldSave()
		{
			AddNewSprite();
			viewModel.UIName = "SceneWithAButton";
			viewModel.UiEditorScene.SaveUI("");
			Assert.AreEqual(1, ((MockService)viewModel.service).NumberOfMessagesSent);
		}

		private void AddNewSprite()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingImage(true);
			viewModel.UiEditorScene.controlAdder.AddImage(new Vector2D(0.5f, 0.5f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add("NewSprite0");
		}

		[Test]
		public void CannotSaveUIIfNoControlsWereAdded()
		{
			viewModel.UIName = "SceneWithAButton";
			bool saved = false;
			mockService.ContentUpdated += (type, name) => //ncrunch: no coverage start
			{
				saved = CheckNameAndTypeOfUpdate(type, name);
			}; //ncrunch: no coverage end
			viewModel.UiEditorScene.SaveUI("");
			Assert.IsFalse(saved);
		}

		[Test]
		public void WillNotLoadNonExistingScene()
		{
			viewModel.UIName = "NoDataScene";
			bool saved = false;
			mockService.ContentUpdated += (type, name) => //ncrunch: no coverage start
			{
				saved = CheckNameAndTypeOfUpdate(type, name);
			}; //ncrunch: no coverage end
			viewModel.UiEditorScene.SaveUI("");
			Assert.IsFalse(saved);
		}

		//ncrunch: no coverage start
		private static bool CheckNameAndTypeOfUpdate(ContentType type, string name)
		{
			return type == ContentType.Scene && name.Equals("NewUI");
		}	//ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void GridShouldBeDrawn()
		{
			viewModel.GridWidth = 10;
			viewModel.GridHeight = 10;
			viewModel.IsShowingGrid = true;
			Assert.AreEqual(10, viewModel.GridWidth);
			Assert.AreEqual(10, viewModel.GridHeight);
			Assert.IsNotEmpty(viewModel.UiEditorScene.UISceneGrid.LinesInGridList);
		}

		[Test, CloseAfterFirstFrame]
		public void IsShowingGridPropertyShouldBeTrue()
		{
			Assert.AreEqual(true, viewModel.IsShowingGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedSpriteNameListShouldHaveDefaultName()
		{
			AddNewSprite();
			Assert.AreEqual("Picture1", viewModel.SelectedControlNamesInList[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedInterActiveButtonNameListShouldHaveDefaultName()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.UiEditorScene.controlAdder.SetDraggingButton(true);
			viewModel.UiEditorScene.controlAdder.AddButton(new Vector2D(0.4f, 0.4f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add(viewModel.UiEditorScene.UIImagesInList[0]);
			mouse.SetNativePosition(new Vector2D(0.45f, 0.45f));
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			viewModel.ContentText = "TestContentText";
			Assert.AreEqual("Button1", viewModel.SelectedControlNamesInList[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void ControlLayerPropertyShouldBeZero()
		{
			Assert.AreEqual(0, viewModel.ControlLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void GetEntity2DWidthAndHeight()
		{
			Assert.AreEqual(0.1f, viewModel.Entity2DHeight);
			Assert.AreEqual(0.2f, viewModel.Entity2DWidth);
		}

		[Test, CloseAfterFirstFrame]
		public void ContextPropertyIsEmptyString()
		{
			AddNewSprite();
			Assert.AreEqual("", viewModel.ContentText);
		}

		[Test, CloseAfterFirstFrame]
		public void SetSelectedEntity2D()
		{
			var mouse = Resolve<MockMouse>();
			AddNewSprite();
			AddNewSprite();
			AddNewSprite();
			mouse.SetNativePosition(new Vector2D(0.5f, 0.5f));
			mouse.SetButtonState(MouseButton.Left, State.Pressing);
			Assert.IsNotNull(viewModel.SelectedEntity2DList);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewButton()
		{
			viewModel.UiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			var mouse = Resolve<MockMouse>();
			viewModel.UiEditorScene.controlAdder.SetDraggingButton(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(2, viewModel.SceneNames.Count);
			Assert.IsFalse(viewModel.EnableButtons);
		}

		[Test, CloseAfterFirstFrame]
		public void NewLabelShouldBeAdded()
		{
			var mouse = Resolve<MockMouse>();
			AddLabel();
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(1, viewModel.UiEditorScene.Scene.Controls.Count);
		}

		private void AddLabel()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingLabel(true);
			viewModel.UiEditorScene.controlAdder.AddLabel(new Vector2D(0.5f, 0.5f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add("Label1");
			Assert.AreEqual("Label1", viewModel.SelectedControlNamesInList[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotAddNewLabel()
		{
			var mouse = Resolve<MockMouse>();
			viewModel.UiEditorScene.controlAdder.SetDraggingLabel(false);
			mouse.SetButtonState(MouseButton.Left, State.Releasing);
			Assert.AreEqual(0, viewModel.Scene.Controls.Count);
			Assert.AreEqual(0, viewModel.UIImagesInList.Count);
			Assert.AreEqual(1, viewModel.MaterialList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImage()
		{
			AddNewSprite();
			viewModel.UiEditorScene.controlTransformer.uiMouseCursor.CurrentCursor =
				ControlTransformer.MoveCursor;
			viewModel.IsSnappingToGrid = true;
			var mouse = Resolve<MockMouse>();
			var startPosition = viewModel.SelectedEntity2DList[0].DrawArea.TopLeft;
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			mouse.SetNativePosition(new Vector2D(0.55f, 0.55f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(startPosition, viewModel.SelectedEntity2DList[0].DrawArea.TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveImageInLandscapeGrid()
		{
			AddNewSprite();
			viewModel.UiEditorScene.controlTransformer.uiMouseCursor.CurrentCursor =
				ControlTransformer.MoveCursor;
			viewModel.UIWidth = 100;
			viewModel.UIHeight = 200;
			viewModel.IsSnappingToGrid = true;
			var mouse = Resolve<MockMouse>();
			var startPosition = viewModel.SelectedEntity2DList[0].DrawArea.TopLeft;
			mouse.SetButtonState(MouseButton.Left, State.Pressed);
			mouse.SetNativePosition(new Vector2D(0.55f, 0.55f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(startPosition, viewModel.SelectedEntity2DList[0].DrawArea.TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void IsShowingGridPropertyShouldBeTrueInitially()
		{
			Assert.IsTrue(viewModel.IsShowingGrid);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaterial()
		{
			viewModel.UiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			AddNewButton();
			viewModel.UiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			viewModel.SelectedEntity2DList.Clear();
			viewModel.SelectedEntity2DList.Add(new Sprite(ContentLoader.Load<Material>("material"),
				Rectangle.One));
			viewModel.UiEditorScene.controlMaterialChanger.ChangeMaterial("newMaterial2D");
			Assert.AreEqual("newMaterial2D", viewModel.SelectedEntity2DList[0].Get<Material>().Name);
		}

		private void AddNewButton()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingButton(true);
			viewModel.UiEditorScene.controlAdder.AddButton(new Vector2D(0.5f, 0.5f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add("NewButton0");
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRenderLayer()
		{
			viewModel.UiEditorScene.SelectedEntity2DList.Add(
				viewModel.UiEditorScene.controlAdder.AddNewImageToList(Vector2D.Half,
					viewModel.UiEditorScene));
			viewModel.ChangeRenderLayer(1);
			Assert.AreEqual(1, viewModel.ControlLayer);
			viewModel.ChangeRenderLayer(-50);
			Assert.AreEqual(0, viewModel.ControlLayer);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeRenderLayerWhenNothingIsSelectedWillNotCrash()
		{
			viewModel.SelectedEntity2DList.Clear();
			viewModel.ChangeRenderLayer(1);
			Assert.AreEqual(1, viewModel.ControlLayer);
		}

		[TestCase("10 x 10", 10), TestCase("16 x 16", 16), TestCase("20 x 20", 20),
		 TestCase("24 x 24", 24), TestCase("50 x 50", 50)]
		public void ChangeGrid(string widthAndHeight, int width)
		{
			viewModel.SelectedResolution = widthAndHeight;
			Assert.AreEqual(width.ToString(), viewModel.GridWidth.ToString());
			Assert.AreEqual(widthAndHeight, viewModel.SelectedResolution);
		}

		[Test, CloseAfterFirstFrame]
		public void DeletingAControlAndAddingANewWillTakeItsPlace()
		{
			AddNewSprite();
			AddNewSprite();
			viewModel.SelectedControlNamesInList.Add(viewModel.UiEditorScene.UIImagesInList[0]);
			viewModel.UiEditorScene.DeleteSelectedControl("");
			AddNewSprite();
		}

		[Test, CloseAfterFirstFrame]
		public void StartingEditorWillLoadSceneNames()
		{
			Assert.AreEqual(2, viewModel.UiEditorScene.SceneNames.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlName()
		{
			AddNewSprite();
			Assert.AreEqual("Picture1", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewSprite()
		{
			AddNewSprite();
			AddNewSprite();
			CheckNewControlName("Picture2");
			AddNewSprite();
			Assert.AreEqual("Picture2", viewModel.UiEditorScene.UIImagesInList[2]);
		}

		private void CheckNewControlName(string newControlName)
		{
			Assert.AreEqual(newControlName, viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.UiEditorScene.UIImagesInList[0] = null;
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewButton()
		{
			AddNewButton();
			AddNewButton();
			CheckNewControlName("Button2");
			AddNewButton();
			Assert.IsFalse(viewModel.IsInteractiveButton);
			Assert.AreEqual("Button2", viewModel.UiEditorScene.UIImagesInList[2]);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewLabel()
		{
			AddNewLabel();
			AddNewLabel();
			CheckNewControlName("Label2");
			AddNewLabel();
			Assert.AreEqual("Label2", viewModel.UiEditorScene.UIImagesInList[2]);
		}

		private void AddNewLabel()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingLabel(true);
			viewModel.UiEditorScene.controlAdder.AddLabel(new Vector2D(0.5f, 0.5f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add("Label3");
		}

		[Test, CloseAfterFirstFrame]
		public void ChangControlNameAndAddNewSlider()
		{
			AddNewSlider();
			AddNewSlider();
			Assert.AreEqual("Slider2", viewModel.ControlName);
			viewModel.ControlName = "TestName";
			Assert.AreEqual("TestName", viewModel.ControlName);
			viewModel.UiEditorScene.UIImagesInList[0] = null;
			AddNewSlider();
			Assert.AreEqual("Slider2", viewModel.UiEditorScene.UIImagesInList[2]);
		}

		private const string NewSliderId = "Slider1";

		private void AddNewSlider()
		{
			viewModel.UiEditorScene.controlAdder.SetDraggingSlider(true);
			viewModel.UiEditorScene.controlAdder.AddSlider(new Vector2D(0.5f, 0.5f),
				viewModel.UiEditorScene);
			viewModel.SelectedControlNamesInList.Add(NewSliderId);
		}

		[Test]
		public void DeleteSelectedContentFromWpf()
		{
			AddNewSlider();
			Assert.AreEqual(NewSliderId, viewModel.ControlName);
			var selectedControlNames = new List<string>();
			viewModel.DeleteSelectedContent("");
			selectedControlNames.Add(NewSliderId);
			viewModel.DeleteSelectedContent("");
			Assert.IsFalse(viewModel.UiEditorScene.UIImagesInList.Contains(NewSliderId));
		}

		[Test]
		public void AddNewResolutionToResolutionList()
		{
			AddResolutionToList();
			Assert.AreEqual(6, viewModel.ResolutionList.Count);
			viewModel.NewGridHeight = 0;
			viewModel.AddNewResolution("");
			Assert.AreEqual(6, viewModel.ResolutionList.Count);
		}

		private void AddResolutionToList()
		{
			viewModel.NewGridWidth = 10;
			viewModel.NewGridHeight = 20;
			viewModel.AddNewResolution("");
			viewModel.AddNewResolution("");
		}

		[Test]
		public void CannotHaveMoreThan10SizesInResolutionList()
		{
			AddResolutionToList();
			Add6Resolutions();
			Assert.AreEqual(10, viewModel.ResolutionList.Count);
		}

		private void Add6Resolutions()
		{
			for (int i = 0; i < 6; i++)
			{
				viewModel.NewGridWidth = i;
				viewModel.NewGridHeight = i * 2;
				viewModel.AddNewResolution("");
			}
		}

		[Test]
		public void ChangeSelectedNameFromTheHierachyList()
		{
			AddNewSlider();
			Assert.AreEqual(NewSliderId, viewModel.SelectedControlNamesInList[0]);
			var selectedContentNames = new List<string>();
			selectedContentNames.Add("Button0");
			viewModel.SetSelectedNamesFromHierachy(selectedContentNames);
			Assert.AreEqual("Button0", viewModel.SelectedControlNamesInList[0]);
		}

		[Test]
		public void ChangeSelectedIndexFromTheHierachyList()
		{
			AddNewSlider();
			viewModel.SetSelectedIndexFromHierachy(1);
			Assert.AreEqual(1, viewModel.ControlListIndex);
		}

		[Test]
		public void ChangingTopMarginWillChangeBottomMargin()
		{
			AddNewSlider();
			viewModel.TopMargin = 0.2f;
			Assert.AreEqual(0.23f, viewModel.BottomMargin);
		}

		[Test]
		public void ChangingBottomMarginWillChangeTopMargin()
		{
			AddNewSlider();
			viewModel.BottomMargin = 0.2f;
			Assert.AreEqual(0.17f, viewModel.TopMargin);
		}

		[Test]
		public void ChangingLeftMarginWillChangeRightMargin()
		{
			AddNewSlider();
			viewModel.LeftMargin = 0.2f;
			Assert.AreEqual(0.35f, viewModel.RightMargin, 0.01f);
		}

		[Test]
		public void ChangingRightMarginWillChangeLeftMargin()
		{
			AddNewSlider();
			viewModel.RightMargin = 0.2f;
			Assert.AreEqual(0.05f, viewModel.LeftMargin, 0.01f);
		}

		[Test]
		public void ChangingHorizontalAllignment()
		{
			AddNewSlider();
			viewModel.HorizontalAllignment = null;
			viewModel.HorizontalAllignment = "Left";
			Assert.AreEqual(0.4f, viewModel.LeftMargin, 0.01f);
			viewModel.HorizontalAllignment = "Right";
			Assert.AreEqual(0.4f, viewModel.LeftMargin, 0.01f);
			viewModel.HorizontalAllignment = "Center";
			Assert.AreEqual(0.425f, viewModel.LeftMargin, 0.01f);
			Assert.AreEqual("Center", viewModel.HorizontalAllignment);
		}

		[Test]
		public void ChangingVerticalAllignment()
		{
			AddNewSlider();
			viewModel.VerticalAllignment = "";
			viewModel.VerticalAllignment = "Top";
			Assert.AreEqual(0.45f, viewModel.TopMargin, 0.01f);
			viewModel.VerticalAllignment = "Bottom";
			Assert.AreEqual(0.45f, viewModel.TopMargin, 0.01f);
			viewModel.VerticalAllignment = "Center";
			Assert.AreEqual(0.485f, viewModel.TopMargin, 0.01f);
			Assert.AreEqual("Center", viewModel.VerticalAllignment);
		}

		[Test]
		public void ChangeSelectedButtonToInterActiveButtonAndBack()
		{
			AddNewButton();
			viewModel.IsInteractiveButton = true;
			Assert.AreEqual(typeof(InteractiveButton), viewModel.SelectedEntity2DList[0].GetType());
			viewModel.IsInteractiveButton = false;
			Assert.AreEqual(typeof(Button), viewModel.SelectedEntity2DList[0].GetType());
		}

		[Test]
		public void DeletingAControlWillSelectTheNextControl()
		{
			AddNewSlider();
			AddNewSlider();
			AddNewSlider();
			AddNewSlider();
			viewModel.UiEditorScene.CanDeleteSelectedControl = true;
			viewModel.SelectedEntity2DList[0] = viewModel.UiEditorScene.Scene.Controls[1];
			viewModel.UiEditorScene.DeleteSelectedControl("");
			Assert.AreEqual(viewModel.SelectedEntity2DList[0], viewModel.UiEditorScene.Scene.Controls[1]);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeControlWidthAndHeight()
		{
			AddNewSlider();
			viewModel.SelectedEntity2DList.Add(viewModel.UiEditorScene.Scene.Controls[0]);
			viewModel.Entity2DWidth = 0.2f;
			viewModel.Entity2DHeight = 0.4f;
			Assert.AreEqual(0.2f, viewModel.SelectedEntity2DList[0].DrawArea.Width);
			Assert.AreEqual(0.4f, viewModel.SelectedEntity2DList[0].DrawArea.Height);
		}

		[Test, CloseAfterFirstFrame]
		public void GetDefaultMaterials()
		{
			AddNewSlider();
			viewModel.SelectedEntity2DList.Add(viewModel.UiEditorScene.Scene.Controls[0]);
			Assert.AreEqual("<GeneratedCustomMaterial:R=128, G=128, B=128, A=255>",
				viewModel.SelectedMaterial);
			viewModel.SelectedMaterial = "";
			Assert.AreEqual("<GeneratedCustomMaterial:R=165, G=165, B=165, A=255>",
				viewModel.SelectedHoveredMaterial);
			viewModel.SelectedHoveredMaterial = "";
			Assert.AreEqual("<GeneratedCustomMaterial:R=165, G=202, B=255, A=255>",
				viewModel.SelectedPressedMaterial);
			viewModel.SelectedPressedMaterial = "";
			Assert.AreEqual("<GeneratedCustomMaterial:R=89, G=89, B=89, A=255>",
				viewModel.SelectedDisabledMaterial);
			viewModel.SelectedDisabledMaterial = "";
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeUIWidthAndHeight()
		{
			AddNewSlider();
			viewModel.UIWidth = 500;
			viewModel.UIHeight = 500;
			Assert.AreEqual(50, viewModel.UiEditorScene.UISceneGrid.LinesInGridList.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void FromSelectingInContentManager()
		{
			viewModel.UIName = "SceneWithAButton";
			viewModel.LoadScene();
			Assert.AreEqual(1, viewModel.Scene.Controls.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CanGenerateGenerateSourceCodeAfterCheckingCheckBox()
		{
			viewModel.CanGenerateSourceCode = true;
			viewModel.UiEditorScene.SaveUI("");
			Assert.AreEqual(true, viewModel.CanGenerateSourceCode);
		}

		[Test, CloseAfterFirstFrame]
		public void ActivatingSceneActivatesTheControls()
		{
			viewModel.Scene.Add(new Button(new Rectangle()));
			foreach (var control in viewModel.Scene.Controls)
				control.IsActive = false;
			viewModel.ActivateHiddenScene();
			Assert.AreEqual(true, viewModel.Scene.Controls[0].IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingSelectedIndexWillChangeSelectedName()
		{
			AddNewButton();
			AddNewButton();
			Assert.AreEqual(0, viewModel.SelectedIndex);
			Assert.AreEqual("Button1", viewModel.SelectedName);
			viewModel.SelectedName = "Test";
			viewModel.SelectedIndex = 1;
			Assert.AreEqual(1, viewModel.SelectedIndex);
			Assert.AreEqual("Button2", viewModel.SelectedName);
		}
	}
}