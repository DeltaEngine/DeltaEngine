using System.Collections.Generic;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Helper class to map all the VkKey keys to our Key enumeration.
	/// </summary>
	internal static class KeyboardKeyMapper
	{
		//ncrunch: no coverage start
		static KeyboardKeyMapper()
		{
			AddCursorKeys();
			AddLeftRightKeys();
			AddNumPadKeys();
			AddFunctionKeys();
			AddMiscKeys();
		}

		private static void AddCursorKeys()
		{
			KeyMap.Add(0x25, Key.CursorLeft);
			KeyMap.Add(0x26, Key.CursorUp);
			KeyMap.Add(0x27, Key.CursorRight);
			KeyMap.Add(0x28, Key.CursorDown);
		}

		private static void AddLeftRightKeys()
		{
			KeyMap.Add(0x11, Key.LeftControl);
			KeyMap.Add(0xA2, Key.LeftControl);
			KeyMap.Add(0xA3, Key.RightControl);
			KeyMap.Add(0x10, Key.LeftShift);
			KeyMap.Add(0xA0, Key.LeftShift);
			KeyMap.Add(0xA1, Key.RightShift);
			KeyMap.Add(0x5B, Key.LeftWindows);
			KeyMap.Add(0x5C, Key.RightWindows);
			KeyMap.Add(0x12, Key.LeftAlt);
			KeyMap.Add(0xA4, Key.LeftAlt);
			KeyMap.Add(0xA5, Key.RightAlt);
		}

		private static void AddNumPadKeys()
		{
			KeyMap.Add(0x60, Key.NumPad0);
			KeyMap.Add(0x61, Key.NumPad1);
			KeyMap.Add(0x62, Key.NumPad2);
			KeyMap.Add(0x63, Key.NumPad3);
			KeyMap.Add(0x64, Key.NumPad4);
			KeyMap.Add(0x65, Key.NumPad5);
			KeyMap.Add(0x66, Key.NumPad6);
			KeyMap.Add(0x67, Key.NumPad7);
			KeyMap.Add(0x68, Key.NumPad8);
			KeyMap.Add(0x69, Key.NumPad9);
		}

		private static void AddFunctionKeys()
		{
			KeyMap.Add(0x70, Key.F1);
			KeyMap.Add(0x71, Key.F2);
			KeyMap.Add(0x72, Key.F3);
			KeyMap.Add(0x73, Key.F4);
			KeyMap.Add(0x74, Key.F5);
			KeyMap.Add(0x75, Key.F6);
			KeyMap.Add(0x76, Key.F7);
			KeyMap.Add(0x77, Key.F8);
			KeyMap.Add(0x78, Key.F9);
			KeyMap.Add(0x79, Key.F10);
			KeyMap.Add(0x7A, Key.F11);
			KeyMap.Add(0x7B, Key.F12);
		}

		private static void AddMiscKeys()
		{
			KeyMap.Add(0x08, Key.Backspace);
			KeyMap.Add(0x09, Key.Tab);
			KeyMap.Add(0x0D, Key.Enter);
			KeyMap.Add(0x13, Key.Pause);
			KeyMap.Add(0x1B, Key.Escape);
			KeyMap.Add(0x20, Key.Space);
			KeyMap.Add(0x23, Key.End);
			KeyMap.Add(0x24, Key.Home);
			KeyMap.Add(0x2D, Key.Insert);
			KeyMap.Add(0x2E, Key.Delete);
			KeyMap.Add(0x6A, Key.Multiply);
			KeyMap.Add(0x6B, Key.Add);
			KeyMap.Add(0x6C, Key.Separator);
			KeyMap.Add(0x6D, Key.Subtract);
			KeyMap.Add(0x6E, Key.Decimal);
			KeyMap.Add(0x6F, Key.Divide);
			KeyMap.Add(0x90, Key.NumLock);
			KeyMap.Add(0x91, Key.Scroll);
			KeyMap.Add(0xBF, Key.Question);
			KeyMap.Add(0xC0, Key.Tilde);
		}

		private static readonly Dictionary<int, Key> KeyMap = new Dictionary<int, Key>();

		public static Key Translate(int key)
		{
			return KeyMap.ContainsKey(key) ? KeyMap[key] : (Key)key;
		}
	}
}