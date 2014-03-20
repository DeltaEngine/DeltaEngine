using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseTapTriggerTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void Tap()
		{
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			bool wasTapped = false;
			new Command(() => wasTapped = true).Add(new MouseTapTrigger(MouseButton.Left));
			SetMouseState(mouse, State.Pressing, Vector2D.Half);
			Assert.IsFalse(wasTapped);
			SetMouseState(mouse, State.Releasing, Vector2D.Half);
			Assert.IsTrue(wasTapped);
		}

		private void SetMouseState(MockMouse mouse, State state, Vector2D position)
		{
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void CreateMouseTapTriggerBySendingButtonName()
		{
			var trigger = new MouseTapTrigger(MouseButton.Middle.ToString());
			Assert.AreEqual(MouseButton.Middle, trigger.Button);
		}

	}
}
