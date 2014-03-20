using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlAdderTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateControlAdder()
		{
			controlAdder = new ControlAdder();
			uiEditorViewModel = new UIEditorViewModel(new MockService("", ""));
			uiControl = new UIControl();
		}

		private ControlAdder controlAdder;
		private UIEditorViewModel uiEditorViewModel;
		private UIControl uiControl;

		[Test]
		public void AddNewButtonToScene()
		{
			var button = new Button(Rectangle.One);
			CheckTypeOfNewControl(button, "Button2");
			Assert.IsTrue(uiEditorViewModel.CanSaveScene);
		}

		private void CheckTypeOfNewControl(Control control, string controlName)
		{
			uiEditorViewModel.UiEditorScene.Scene.Add(control);
			var controls = uiEditorViewModel.UiEditorScene.Scene.Controls;
			Assert.AreEqual(controlName, (controls[controls.Count - 1] as Control).Name);
		}

		[Test]
		public void InterActiveButtons()
		{
			var interactiveButton = new InteractiveButton(Rectangle.One);
			CheckTypeOfNewControl(interactiveButton, "InteractiveButton1");
		}

		[Test]
		public void AddNewSliderToScene()
		{
			var slider = new Slider(Rectangle.One);
			slider.AddTag("slider");
			CheckTypeOfNewControl(slider, "Slider1");
		}

		[Test]
		public void AddNewLabelToScene()
		{
			var label = new Label(Rectangle.One);
			CheckTypeOfNewControl(label, "Label1");
		}

		[Test]
		public void AddNewSpriteToScene()
		{
			var picture = new Picture(new Theme(), new Material(ShaderFlags.Position2DColoredTextured, "deltalogo"),
				Rectangle.One);
			CheckTypeOfNewControl(picture, "Picture1");
		}

		[Test]
		public void CreateCenteredPicture()
		{
			CreateCenteredControl("Image");
		}

		private void CreateCenteredControl(string controlType)
		{
			uiEditorViewModel.SelectedEntity2DList.Clear();
			controlAdder.CreateCenteredControl(controlType, uiEditorViewModel.UiEditorScene);
			var controls = uiEditorViewModel.UiEditorScene.Scene.Controls;
			Assert.AreEqual(uiEditorViewModel.SelectedEntity2DList[0], controls[controls.Count - 1]);
		}

		[Test]
		public void CreateCenteredButton()
		{
			CreateCenteredControl("Button");
		}

		[Test]
		public void CreateCenteredLabel()
		{
			CreateCenteredControl("Label");
		}

		[Test]
		public void CreateCenteredSlider()
		{
			CreateCenteredControl("Slider");
		}

		[Test]
		public void GivingAWrongControllWillJustReturn()
		{
			uiEditorViewModel.SelectedEntity2DList.Clear();
			controlAdder.CreateCenteredControl("NoControl", uiEditorViewModel.UiEditorScene);
			Assert.AreEqual(0, uiEditorViewModel.SelectedEntity2DList.Count);
		}
	}
}