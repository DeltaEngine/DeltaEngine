using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	[RequiresSTA]
	public class ControlTransformerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void UIEditorViewModel()
		{
			uiEditor = new UIEditorViewModel(new MockService("", ""));
			uiEditor.Scene.Controls.Add(new Button(Rectangle.One));
			uiEditor.SelectedEntity2DList.Clear();
			uiEditor.SelectedEntity2DList.Add(uiEditor.Scene.Controls[1]);
			ControlTransformer.usedControl = (Control)uiEditor.Scene.Controls[1];
		}

		public UIEditorViewModel uiEditor;

		[Test]
		public void CursorChangesToMoveWhenHoverOverControl()
		{
			uiEditor.UiEditorScene.CheckIfCanTransformControl(Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void CursorChangesToScaleWhenHoverOverEdgeOfControlControl()
		{
			uiEditor.UiEditorScene.CheckIfCanTransformControl(new Vector2D(0.005f, 0.005f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void CursorChangesToRoateWhenHoverNextToTheEdgeOfControlControl()
		{
			uiEditor.UiEditorScene.CheckIfCanTransformControl(new Vector2D(-0.005f, -0.005f));
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void RotateControlClockWise()
		{
			uiEditor.UiEditorScene.controlTransformer.RotateControl(new Vector2D(0.5f, 0.0f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(0, uiEditor.Scene.Controls[1].Rotation, 0.001f);
			uiEditor.UiEditorScene.controlTransformer.RotateControl(new Vector2D(1.0f, 0.5f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(90, uiEditor.Scene.Controls[1].Rotation, 0.001f);
		}

		[Test]
		public void RotateControlCounterClockWise()
		{
			uiEditor.UiEditorScene.controlTransformer.RotateControl(new Vector2D(0.5f, 0.0f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(0, uiEditor.Scene.Controls[1].Rotation, 0.001f);
			uiEditor.UiEditorScene.controlTransformer.RotateControl(new Vector2D(0.0f, 0.5f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(-90, uiEditor.Scene.Controls[1].Rotation, 0.001f);
		}

		[Test]
		public void ScaleControl()
		{
			uiEditor.UiEditorScene.controlTransformer.ScaleControl(new Vector2D(0.5f, 0.0f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(1, uiEditor.Scene.Controls[1].DrawArea.Width, 0.001f);
			uiEditor.UiEditorScene.controlTransformer.ScaleControl(new Vector2D(1.0f, 0.0f),
				uiEditor.SelectedEntity2DList);
			Assert.AreEqual(2f, uiEditor.Scene.Controls[1].DrawArea.Width, 0.001f);
		}

		[Test]
		public void CreateBoundingBoxOf2Controls()
		{
			uiEditor.Scene.Clear();
			uiEditor.Scene.Controls.Add(new Button(new Rectangle(0.0f, 0.0f, 0.4f, 0.2f)));
			uiEditor.Scene.Controls.Add(new Button(new Rectangle(0.8f, 0.8f, 0.2f, 0.2f)));
			uiEditor.SelectedEntity2DList.Clear();
			uiEditor.SelectedEntity2DList.Add(uiEditor.Scene.Controls[0]);
			uiEditor.SelectedEntity2DList.Add(uiEditor.Scene.Controls[1]);
			Rectangle rect =
				uiEditor.UiEditorScene.controlTransformer.CalculateBoundingBox(
					uiEditor.SelectedEntity2DList);
			Assert.AreEqual(rect.Center, Vector2D.Half);
		}

		[Test]
		public void CreateBoundingBoxOfRoatatedControls()
		{
			uiEditor.Scene.Clear();
			var control = new Button(new Rectangle(0.0f, 0.0f, 0.4f, 0.2f));
			control.Rotation = 45;
			uiEditor.Scene.Controls.Add(control);
			uiEditor.Scene.Controls.Add(new Button(new Rectangle(0.8f, 0.8f, 0.2f, 0.2f)));
			uiEditor.SelectedEntity2DList.Clear();
			uiEditor.SelectedEntity2DList.Add(uiEditor.Scene.Controls[0]);
			uiEditor.SelectedEntity2DList.Add(uiEditor.Scene.Controls[1]);
			Rectangle rect =
				uiEditor.UiEditorScene.controlTransformer.CalculateBoundingBox(uiEditor.SelectedEntity2DList);
			Assert.AreNotEqual(rect.Center, Vector2D.Half);
		}
	}
}