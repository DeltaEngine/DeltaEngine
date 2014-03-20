using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void SetUp()
		{
			isClicked = false;
			if (StackTraceExtensions.StartedFromNUnitConsoleButNotFromNCrunch)
				Assert.Ignore(); //ncrunch: no coverage
		}

		private bool isClicked;

		[Test, CloseAfterFirstFrame]
		public void TestLeftMouseButtonClickPassingAction()
		{
			new Command(() => isClicked = true).Add(new MouseButtonTrigger(MouseButton.Left,
				State.Pressed));
			TestCommand();
		}

		private void TestCommand()
		{
			Assert.IsFalse(isClicked);
			var mockMouse = Resolve<Mouse>() as MockMouse;
			if (mockMouse == null)
				return; //ncrunch: no coverage
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(isClicked);
			Assert.IsTrue(mockMouse.IsAvailable);
			mockMouse.Dispose();
		}

		[Test, CloseAfterFirstFrame]
		public void TestLeftMouseButtonClickPassingPositionAction()
		{
			new Command(delegate(Vector2D point) { isClicked = true; }).Add(
				new MouseButtonTrigger(MouseButton.Left, State.Pressed));
			TestCommand();
		}

		[Test, CloseAfterFirstFrame]
		public void GetButtonState()
		{
			var mouse = Resolve<Mouse>();
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Left));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Middle));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.Right));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X1));
			Assert.AreEqual(State.Released, mouse.GetButtonState(MouseButton.X2));
		}

		[Test, CloseAfterFirstFrame]
		public void SetButtonState()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			if (mockMouse == null)
				return; //ncrunch: no coverage
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			Assert.AreEqual(State.Pressed, mockMouse.GetButtonState(MouseButton.Left));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			Assert.AreEqual(State.Pressed, mockMouse.GetButtonState(MouseButton.Middle));
			mockMouse.SetButtonState(MouseButton.Right, State.Pressed);
			Assert.AreEqual(State.Pressed, mockMouse.GetButtonState(MouseButton.Right));
			mockMouse.SetButtonState(MouseButton.X1, State.Pressed);
			Assert.AreEqual(State.Pressed, mockMouse.GetButtonState(MouseButton.X1));
			mockMouse.SetButtonState(MouseButton.X2, State.Pressed);
			Assert.AreEqual(State.Pressed, mockMouse.GetButtonState(MouseButton.X2));
		}

		[Test, CloseAfterFirstFrame]
		public void ScrollWheelUpAndDown()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			if (mockMouse == null)
				return; //ncrunch: no coverage
			Assert.AreEqual(0, mockMouse.ScrollWheelValue);
			mockMouse.ScrollUp();
			Assert.AreEqual(1, mockMouse.ScrollWheelValue);
			mockMouse.ScrollDown();
			Assert.AreEqual(0, mockMouse.ScrollWheelValue);
		}

		[Test]
		public void ShowScrollWheelValue()
		{
			var mouse = Resolve<Mouse>();
			var text = new FontText(Font.Default, "Scroll Mouse Wheel!", Rectangle.One);
			new Command(() => text.Text = "Scroll Wheel Value: " + mouse.ScrollWheelValue).Add(
				new MouseZoomTrigger());
		}
	}
}