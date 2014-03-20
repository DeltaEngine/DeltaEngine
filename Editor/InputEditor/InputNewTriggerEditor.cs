using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputNewTriggerEditor
	{
		//ncrunch: no coverage start
		public void CreateNewTriggerBox(CommandList commandList, string selectedCommandInList,
			TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			availableCommands = commandList;
			selectedCommand = selectedCommandInList;
			FillBoxWithInputKeysWithKeyEnum();
			FillBoxWithInputTypeEnum();
			FillBoxWithInputStateEnum();
			SetSelectedItem();
		}

		private TriggerLayoutView trigger;
		private CommandList availableCommands;
		private string selectedCommand;

		private void FillBoxWithInputKeysWithKeyEnum()
		{
			Array enumValues = Enum.GetValues(typeof(Key));
			foreach (var value in enumValues)
				AddKeyEnumValueToKeyComboBox(value);
		}

		private void AddKeyEnumValueToKeyComboBox(object value)
		{
			string enumvalue = Enum.GetName(typeof(Key), value);
			trigger.TriggerKey.Items.Add(enumvalue);
		}

		private void FillBoxWithInputTypeEnum()
		{
			Array enumValues = Enum.GetValues(typeof(InputType));
			foreach (var value in enumValues)
				AddEnumValueToTypeComboBox(value);
		}

		private enum InputType
		{
			Keyboard,
			Gamepad,
			MouseButton,
			MouseDragAndDrop,
			MouseHold,
			MouseHover,
			MouseMovement,
			Touchpad
		}

		private void AddEnumValueToTypeComboBox(object value)
		{
			string enumvalue = Enum.GetName(typeof(InputType), value);
			trigger.TriggerType.Items.Add(enumvalue);
		}

		private void FillBoxWithInputStateEnum()
		{
			Array enumValues = Enum.GetValues(typeof(State));
			foreach (var value in enumValues)
				AddEnumValueToStateComboBox(value);
		}

		private void AddEnumValueToStateComboBox(object value)
		{
			string enumvalue = Enum.GetName(typeof(State), value);
			trigger.TriggerState.Items.Add(enumvalue);
		}

		private void SetSelectedItem()
		{
			List<Trigger> triggerList = availableCommands.GetAllTriggers(selectedCommand);
			bool foundFreeKey = false;
			foreach (Key key in Enum.GetValues(typeof(Key)))
				foundFreeKey = CheckWhichKeyToUse(foundFreeKey, triggerList, key);
		}

		private bool CheckWhichKeyToUse(bool foundFreeKey, IEnumerable<Trigger> triggerList, Key key)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in triggerList)
				keyAlreadyUsed = CHeckIfKeyIsALreadyUsed(newTrigger, key, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			trigger.TriggerKey.SelectedItem = key.ToString();
			trigger.TriggerState.SelectedItem = State.Pressed.ToString();
			trigger.TriggerType.SelectedItem = InputType.Keyboard.ToString();
			availableCommands.AddTrigger(selectedCommand, key, State.Pressed);
			return true;
		}

		private static bool CHeckIfKeyIsALreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(KeyTrigger))
				return keyAlreadyUsed; //ncrunch: no coverage
			var keyTrigger = (KeyTrigger)newTrigger;
			if (keyTrigger.Key.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		public void SetKeyTrigger(Key newKey, State newState, TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithInputKeysWithKeyEnum();
			FillBoxWithInputTypeEnum();
			FillBoxWithInputStateEnum();
			trigger.TriggerType.SelectedItem = InputType.Keyboard.ToString();
			trigger.TriggerKey.SelectedItem = newKey.ToString();
			trigger.TriggerState.SelectedItem = newState.ToString();
		}

		public void SetMouseButtonTrigger(MouseButton newButton, State newState,
			TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithMouseEnum();
			FillBoxWithInputTypeEnum();
			FillBoxWithInputStateEnum();
			trigger.TriggerType.SelectedItem = InputType.MouseButton.ToString();
			trigger.TriggerKey.SelectedItem = newButton.ToString();
			trigger.TriggerState.SelectedItem = newState.ToString();
		}

		private void FillBoxWithMouseEnum()
		{
			Array enumValues = Enum.GetValues(typeof(MouseButton));
			foreach (var value in enumValues)
				AddMouseEnumValueToButtonComboBox(value);
		}

		private void AddMouseEnumValueToButtonComboBox(object value)
		{
			string enumvalue = Enum.GetName(typeof(MouseButton), value);
			trigger.TriggerKey.Items.Add(enumvalue);
		}

		public void SetMouseDragDropTrigger(MouseButton newButton, TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithMouseEnum();
			FillBoxWithInputTypeEnum();
			trigger.TriggerType.SelectedItem = InputType.MouseDragAndDrop.ToString();
			trigger.TriggerKey.SelectedItem = newButton.ToString();
			trigger.TriggerState.SelectedItem = "";
		}

		public void SetMouseHoldTrigger(MouseButton newButton, TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithMouseEnum();
			FillBoxWithInputTypeEnum();
			trigger.TriggerType.SelectedItem = InputType.MouseHold.ToString();
			trigger.TriggerKey.SelectedItem = newButton.ToString();
			trigger.TriggerState.SelectedItem = "";
		}

		public void SetMouseHoverTrigger(TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithInputTypeEnum();
			trigger.TriggerType.SelectedItem = InputType.MouseHover.ToString();
			trigger.TriggerKey.SelectedItem = "";
			trigger.TriggerState.SelectedItem = "";
		}

		public void SetMouseMovementTrigger(TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithInputTypeEnum();
			trigger.TriggerType.SelectedItem = InputType.MouseMovement.ToString();
			trigger.TriggerKey.SelectedItem = "";
			trigger.TriggerState.SelectedItem = "";
		}

		public void SetGamePadTrigger(GamePadButton newButton, State newState,
			TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			FillBoxWithGamePadEnum();
			FillBoxWithInputTypeEnum();
			FillBoxWithInputStateEnum();
			trigger.TriggerType.SelectedItem = InputType.Gamepad.ToString();
			trigger.TriggerKey.SelectedItem = newButton.ToString();
			trigger.TriggerState.SelectedItem = newState.ToString();
		}

		private void FillBoxWithGamePadEnum()
		{
			Array enumValues = Enum.GetValues(typeof(GamePadButton));
			foreach (var value in enumValues)
				AddGamePadEnumvalueToButtonComboBox(value);
		}

		private void AddGamePadEnumvalueToButtonComboBox(object value)
		{
			string enumvalue = Enum.GetName(typeof(GamePadButton), value);
			trigger.TriggerKey.Items.Add(enumvalue);
		}

		public void SetTouchPadTrigger(State newState, TriggerLayoutView newTrigger)
		{
			trigger = newTrigger;
			trigger.TriggerKey.Items.Clear();
			FillBoxWithInputTypeEnum();
			FillBoxWithInputStateEnum();
			trigger.TriggerType.SelectedItem = InputType.Touchpad.ToString();
			trigger.TriggerState.SelectedItem = newState.ToString();
		}
	}
}