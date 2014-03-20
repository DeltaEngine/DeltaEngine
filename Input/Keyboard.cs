using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Keyboard device (virtual or real).
	/// </summary>
	public abstract class Keyboard : InputDevice
	{
		public State GetKeyState(Key key)
		{
			return keyboardStates[(int)key];
		}

		protected readonly State[] keyboardStates = new State[(int)Key.NumberOfKeys];

		public override void Update(IEnumerable<Entity> entities)
		{
			if (!IsAvailable)
				return; //ncrunch: no coverage
			foreach (Entity entity in entities)
			{
				ProcessKeyTrigger(entity);
				ProcessInputHandling(entity);
			}
			newlyPressedKeys.Clear();
			UpdateKeyStates();
		}

		private void ProcessKeyTrigger(Entity entity)
		{
			var trigger = entity as KeyTrigger;
			if (trigger != null)
				trigger.HandleWithKeyboard(this);
		}

		private void ProcessInputHandling(Entity entity)
		{
			var keyEntity = entity as KeyboardControllable;
			if (keyEntity != null)
				keyEntity.UpdateTextFromKeyboardInput(HandleInput);
		}

		public string HandleInput(string inputText)
		{
			foreach (var key in newlyPressedKeys)
				inputText = HandleInputForKey(inputText, key);
			newlyPressedKeys.Clear();
			return inputText;
		}

		protected readonly List<Key> newlyPressedKeys = new List<Key>();

		protected abstract void UpdateKeyStates();

		private string HandleInputForKey(string inputText, Key key)
		{
			if (key >= Key.D0 && key <= Key.D9)
				return inputText + key.ToString()[1].ToString(CultureInfo.InvariantCulture);
			if (key >= Key.NumPad0 && key <= Key.NumPad9)
				return inputText + key.ToString()[6].ToString(CultureInfo.InvariantCulture);
			if (key >= Key.A && key <= Key.Z)
				if (KeyNeedsToBeCapitalized())
					return inputText + key;
				else
					return inputText + (char)((int)key + 32);
			if (key == Key.Space)
				return inputText + " ";
			if (key == Key.Decimal || key == Key.Period)
				return inputText + ".";
			if (key == Key.Comma)
				return inputText + ",";
			if (key == Key.Backspace && inputText.Length > 0)
				return inputText.Substring(0, inputText.Length - 1);
			return inputText;
		}

		private bool KeyNeedsToBeCapitalized()
		{
			bool isAnyShiftPressed = keyboardStates[(int)Key.LeftShift] >= State.Pressing ||
				keyboardStates[(int)Key.RightShift] >= State.Pressing;
			return IsCapsLocked ? !isAnyShiftPressed : isAnyShiftPressed;
		}

		protected abstract bool IsCapsLocked { get; }

		public override bool IsAvailable { get; protected set; }
	}
}