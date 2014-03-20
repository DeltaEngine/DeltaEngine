using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Editor.UIEditor.Tests
{
	public class ControlChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			controlChanger = new ControlChanger();
			var service = new MockService("", "");
			uiEditorScene = new UIEditorScene(service);
		}

		private ControlChanger controlChanger;
		private UIEditorScene uiEditorScene;

		[Test]
		public void SetHeightOfSelectedControler()
		{
			SetSelectedControl();
			controlChanger.SetHeight(0.5f, uiEditorScene);
			Assert.AreEqual(0.5f, uiEditorScene.SelectedEntity2DList[0].DrawArea.Height);
		}

		private void SetSelectedControl()
		{
			uiEditorScene.uiControl.isClicking = false;
			uiEditorScene.uiControl.contentText = "Hello";
			uiEditorScene.SelectedEntity2DList.Add(new Button(Rectangle.One));
		}

		[Test]
		public void SetWidthOfSelectedControler()
		{
			SetSelectedControl();
			controlChanger.SetWidth(0.5f, uiEditorScene);
			Assert.AreEqual(0.5f, uiEditorScene.SelectedEntity2DList[0].DrawArea.Width);
		}

		[Test]
		public void CannotSetWidthAndHeightWhenNoControlSelected()
		{
			uiEditorScene.uiControl.isClicking = false;
			uiEditorScene.uiControl.contentText = "Hello";
			controlChanger.SetWidth(0.5f, uiEditorScene);
			controlChanger.SetHeight(0.5f, uiEditorScene);
			Assert.AreEqual(0, uiEditorScene.SelectedEntity2DList.Count);
		}

		[Test]
		public void ChangeTextOfALabel()
		{
			uiEditorScene.SelectedEntity2DList.Add(new Label(Rectangle.One));
			Assert.AreEqual("",((Label)uiEditorScene.SelectedEntity2DList[0]).Text);
			controlChanger.SetContentText("New Text", uiEditorScene);
			Assert.AreEqual("New Text", ((Label)uiEditorScene.SelectedEntity2DList[0]).Text);
		}

		[Test]
		public void CannotSelectControlNameWhenListIsEmpty()
		{
			uiEditorScene.Scene.Controls.Clear();
			var selectedControlList = new List<string>();
			controlChanger.SetSelectedControlNameInList(selectedControlList, uiEditorScene);
			Assert.AreEqual(0, uiEditorScene.SelectedEntity2DList.Count);
		}
	}
}