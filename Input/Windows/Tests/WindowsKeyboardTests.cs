using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class WindowsKeyboardTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateWindowsKeyboard()
		{
			keyboard = new WindowsKeyboard();
		}

		private WindowsKeyboard keyboard;

		[Test, CloseAfterFirstFrame]
		public void UpdateKeyboard()
		{
			var trigger = new KeyTrigger(Key.A);
			keyboard.Update(new[]{trigger});
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeSetsUnavailable()
		{
			keyboard.Dispose();
			Assert.IsFalse(keyboard.IsAvailable);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateKeyStates()
		{
			var mockKeyboard = new MockWindowsKeyboard();
			mockKeyboard.SetPressedKey(Key.A);
			var triggerPressing = new KeyTrigger(Key.A);
			mockKeyboard.Update(new[] { triggerPressing });
			Assert.AreEqual(State.Pressing, mockKeyboard.GetKeyboardState(Key.A));
			mockKeyboard.Update(new[] { triggerPressing });
			Assert.AreEqual(State.Pressed, mockKeyboard.GetKeyboardState(Key.A));
			mockKeyboard.SetReleasedKeys(Key.A);
			mockKeyboard.Update(new[] { new KeyTrigger(Key.A, State.Released) });
			Assert.AreEqual(State.Releasing, mockKeyboard.GetKeyboardState(Key.A));
		}

		private class MockWindowsKeyboard : WindowsKeyboard
		{
			public void SetPressedKey(Key key)
			{
				pressedKeys.Add(key);
			}

			public State GetKeyboardState(Key key)
			{
				return keyboardStates[(int)key];
			}

			public void SetReleasedKeys(Key key)
			{
				releasedKeys.Add(key);
			}
		}
	}
}
