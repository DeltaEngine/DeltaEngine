using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes.EntityDebugger;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.EntityDebugger
{
	internal class EntitySelectorTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			selector = new EntitySelector();
			ellipse = new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Blue);
			mouse = Resolve<Mouse>() as MockMouse;
		}

		private EntitySelector selector;
		private Ellipse ellipse;
		private MockMouse mouse;

		[Test]
		public void BouncingLogosThatCanBeObservedByRightClicking()
		{
			selector.Add(ellipse);
			for (int i = 0; i < NumberOfLogos; i++)
				selector.Add(new BouncingLogo());
		}

		private const int NumberOfLogos = 3;

		[Test]
		public void BouncingLogosThatCanBeEditedByRightClicking()
		{
			selector.EditorMode = EditorMode.Write;
			selector.Add(ellipse);
			for (int i = 0; i < NumberOfLogos; i++)
				selector.Add(new BouncingLogo());
		}

		[Test, CloseAfterFirstFrame]
		public void AddEntity()
		{
			selector.Add(ellipse);
			Assert.AreEqual(1, selector.entities.Count);
			Assert.AreEqual(ellipse, selector.entities[0]);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingEntitySelectsItForReadingAndDoesNotPauseApp()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			selector.Add(ellipse);
			Click(Vector2D.Half);
			Assert.IsTrue(selector.ActiveEditor is EntityReader);
			Assert.IsFalse(Time.IsPaused);
		}

		private void Click(Vector2D position)
		{
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Right, State.Pressing);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingEntityTwiceDisposesOldEditor()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			selector.Add(ellipse);
			Click(Vector2D.Half);
			EntityEditor oldEditor = selector.ActiveEditor;
			Click(Vector2D.Half);
			Assert.AreEqual(0, oldEditor.scene.Controls.Count);
			Assert.AreNotEqual(oldEditor, selector.ActiveEditor);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingOutsideEntityDoesNothing()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			selector.Add(ellipse);
			Click(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
			Assert.IsNull(selector.ActiveEditor);
		}

		// ncrunch: no coverage start
		// Can't run these tests in parallel with any others because they set Time.IsPaused
		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickingEntitySelectsItForWritingAndPausesApp()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			selector.Add(ellipse);
			selector.EditorMode = EditorMode.Write;
			Click(Vector2D.Half);
			Assert.IsTrue(selector.ActiveEditor is EntityWriter);
			Assert.IsTrue(Time.IsPaused);
			Time.IsPaused = false;
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void ClickingOutsideEntityUnpausesApp()
		{
			if (mouse == null)
				return; // ncrunch: no coverage
			Time.IsPaused = true;
			Click(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
			Assert.IsFalse(Time.IsPaused);
		}

		// ncrunch: no coverage end
	}
}