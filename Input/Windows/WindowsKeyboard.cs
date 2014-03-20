using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native keyboard implementation using a windows hook.
	/// </summary>
	public class WindowsKeyboard : Keyboard
	{
		public WindowsKeyboard()
		{
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				hook = new WindowsHook(WindowsHook.KeyboardHookId, HandleProcMessage);//ncrunch: no coverage
			pressedKeys = new List<Key>();
			releasedKeys = new List<Key>();
			InitializeIsAvailable();
		}

		private readonly WindowsHook hook;
		protected readonly List<Key> pressedKeys;
		protected readonly List<Key> releasedKeys;

		private void InitializeIsAvailable()
		{
			IsAvailable = true;
		}

		protected override void UpdateKeyStates()
		{
			for (int i = 0; i < (int)Key.NumberOfKeys; i++)
			{
				keyboardStates[i] = ProcessReleasedAndPressedListsForKey((Key)i);
				if (keyboardStates[i] == State.Pressing)
					newlyPressedKeys.Add((Key)i);
			}
			pressedKeys.Clear();
			releasedKeys.Clear();
		}

		private State ProcessReleasedAndPressedListsForKey(Key key)
		{
			bool isReleased = releasedKeys.Contains(key);
			bool isPressed = pressedKeys.Contains(key);
			return UpdateInputState(keyboardStates[(int)key], isReleased, isPressed);
		}

		private static State UpdateInputState(State previousState, bool isReleased, bool isPressed)
		{
			if (previousState == State.Pressing && isReleased == false)
				return State.Pressed;
			if (isPressed && isReleased == false)
				return previousState != State.Pressed ? State.Pressing : previousState;
			if (isReleased)
				return State.Releasing;
			return previousState == State.Releasing ? State.Released : previousState;
		}

		//ncrunch: no coverage start
		private void HandleProcMessage(IntPtr wParam, IntPtr lParam, int msg)
		{
			Key keyCode = KeyboardKeyMapper.Translate(wParam.ToInt32());
			if (IsKeyPressed(lParam.ToInt32()))
				pressedKeys.Add(keyCode);
			else
				releasedKeys.Add(keyCode);
		}

		private static bool IsKeyPressed(int lParam)
		{
			return ((uint)(lParam & 0x80000000) >> 0xFF) != 1;
		} //ncrunch: no coverage end

		public override void Dispose()
		{
			if (hook != null)
				hook.Dispose(); //ncrunch: no coverage
			IsAvailable = false;
		}

		protected override bool IsCapsLocked
		{
			get { return (((ushort)GetKeyState(0x14)) & 0xffff) != 0; } //ncrunch: no coverage
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true,
			CallingConvention = CallingConvention.Winapi)]
		private static extern short GetKeyState(int keyCode);
	}
}