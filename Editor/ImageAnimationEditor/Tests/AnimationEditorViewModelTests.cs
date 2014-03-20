using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ImageAnimationEditor.Tests
{
	public class AnimationEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			service = new MockService("TestUser", "DeltaEngine.Editor.ImageAnimationEditor.Tests");
			editor = new AnimationEditorViewModel(service);
		}

		private MockService service;
		private AnimationEditorViewModel editor;

		[Test]
		public void MoveImageUpInTheList()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.AddImage("");
			editor.SelectedImageToAdd = "Test2";
			editor.AddImage("");
			editor.MoveImageUp(1);
			Assert.AreEqual("Test2", editor.ImageList[0]);
			editor.MoveImageDown(0);
			Assert.AreEqual("Test1", editor.ImageList[0]);
			editor.MoveImageUp(0);
			editor.MoveImageDown(1);
			service.ChangeProject("LogoApp");
			Assert.AreEqual(1, editor.SelectedIndex);
		}

		[Test]
		public void AddingAndRemovingImagesToList()
		{
			editor.SelectedImageToAdd = "TestImage";
			editor.AddImage("");
			Assert.AreEqual(1,editor.ImageList.Count);
			editor.RemoveImage(editor.SelectedIndex);
			Assert.AreEqual(0, editor.ImageList.Count);
			Assert.IsFalse(editor.IsRemoveEnabled);
			Assert.IsFalse(editor.IsMovingEnabled);
			Assert.IsTrue(editor.IsAddEnabled);
		}

		[Test]
		public void SaveAnimation()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.AddImage("");
			editor.SelectedImageToAdd = "Test2";
			editor.SubImageSize = new Size(-1, 1000);
			editor.AddImage("");
			Assert.IsTrue(editor.IsDisplayingAnimation);
			editor.AnimationName = "TestAnimation";
			editor.SaveAnimation("");
			Assert.IsTrue(editor.CanSaveAnimation);	
		}

		[Test]
		public void SaveSpriteSheet()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.AddImage("");
			editor.AnimationName = "TestAnimation";
			editor.SubImageSize = new Size(1000, 1000);
			editor.SaveAnimation("");
			Assert.IsTrue(editor.CanSaveAnimation);	
		}

		[Test]
		public void HavingNoImageWillDisableSaveButton()
		{
			editor.AnimationName = "TestAnimation";
			editor.SaveAnimation("");
			Assert.IsFalse(editor.CanSaveAnimation);	
		}

		[Test]
		public void LoadAnimation()
		{
			editor.AnimationName = "ImageAnimation";
			Assert.AreEqual(3, editor.ImageList.Count);
		}

		[Test]
		public void LoadSpriteSheet()
		{
			editor.AnimationName = "SpriteSheet";
			Assert.AreEqual(1, editor.ImageList.Count);
		}

		[Test]
		public void CannotAddImageWhenNoImageSelected()
		{
			Assert.AreEqual(0, editor.ImageList.Count);
			editor.AddImage("");
			Assert.AreEqual(0, editor.ImageList.Count);
		}

		[Test]
		public void DisplayImage()
		{
			editor.IsDisplayingImage = true;
			editor.SelectedImageToAdd = editor.LoadedImageList[0];
			editor.AddImage("");
			Assert.AreEqual(1, editor.ImageList.Count);
			Assert.IsTrue(editor.IsFrameSizeEnabled);
		}

		[Test]
		public void ProjectChangeWillResetPlugin()
		{
			editor.SelectedImageToAdd = editor.LoadedImageList[0];
			editor.AddImage("");
			Assert.AreEqual(1, editor.ImageList.Count);
			editor.ResetOnProjectChange();
			Assert.AreEqual(0, editor.ImageList.Count);
			editor.ActivateAnimation();
		}

		[Test]
		public void ButtonStatesShouldInitialyBeFalse()
		{
			Assert.IsFalse(editor.IsRemoveEnabled);
			Assert.IsFalse(editor.IsMovingEnabled);
			Assert.IsFalse(editor.IsAddEnabled);
		}

		[Test]
		public void AfterAddingImagesButtonStatesShouldBeTrue()
		{
			editor.SelectedImageToAdd = "Test1";
			editor.AddImage("");
			editor.AddImage("");
			Assert.IsTrue(editor.IsRemoveEnabled);
			Assert.IsTrue(editor.IsMovingEnabled);
			Assert.IsTrue(editor.IsAddEnabled);
		}
	}
}