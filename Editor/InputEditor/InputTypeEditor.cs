using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputTypeEditor
	{
		//ncrunch: no coverage start
		public InputTypeEditor(InputEditorViewModel inputEditorViewModel)
		{
			this.inputEditorViewModel = inputEditorViewModel;
		}

		private readonly InputEditorViewModel inputEditorViewModel;

		public void ChangeExistingTypeInList(string adding, string key)
		{
			for (int i = 0; i < GetTriggersOfCommand().Count; i++)
				CheckWichTriggerTypeToChange(adding, key, i);
			inputEditorViewModel.UpdateTriggerList(inputEditorViewModel.SelectedCommand);
		}

		private void CheckWichTriggerTypeToChange(string adding, string key, int i)
		{
			var trigger = GetTriggersOfCommand()[i];
			if (trigger.GetType() == typeof(KeyTrigger))
				ChangeTriggerTypeInCommandList(adding, key, trigger);
			else if (trigger.GetType() == typeof(MouseButtonTrigger))
				ChangeMouseTriggerTypeInCommandList(adding, key, trigger); //ncrunch: no coverage
			else if (trigger.GetType() == typeof(MouseDragDropTrigger))
				ChangeMouseDragDropTriggerTypeInCommandList(adding, trigger); //ncrunch: no coverage
			else if (trigger.GetType() == typeof(MouseHoldTrigger))
				ChangeMouseHoldTriggerTypeInCommandList(adding, key, trigger); //ncrunch: no coverage
			else if (trigger.GetType() == typeof(MouseHoverTrigger))
				ChangeMouseHoverTriggerTypeInCommandList(adding, trigger); //ncrunch: no coverage
			else if (trigger.GetType() == typeof(MouseMovementTrigger))
				ChangeMouseMovementTriggerTypeInCommandList(adding, trigger); //ncrunch: no coverage
			else if (trigger.GetType() == typeof(GamePadButtonTrigger))
				ChangeGamePadTriggerTypeInCommandList(adding, key, trigger);
		}

		private List<Trigger> GetTriggersOfCommand()
		{
			return GetCommands().GetAllTriggers(inputEditorViewModel.SelectedCommand);
		}

		private CommandList GetCommands()
		{
			return inputEditorViewModel.availableCommands;
		}

		private void ChangeTriggerTypeInCommandList(string adding, string key, Trigger trigger)
		{
			var newKeyTrigger = (KeyTrigger)trigger;
			if (newKeyTrigger.Key.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		private void CheckWhatTriggerType(string adding, Trigger trigger)
		{
			int index = GetTriggersOfCommand().IndexOf(trigger);
			if (adding == "Keyboard")
				ChanceTriggerToKeyTrigger(index);
			else if (adding == "MouseButton")
				ChangeTriggerToMouseTrigger(index);
			else if (adding == "MouseDragAndDrop")
				ChangeTriggerToMouseDragDropTrigger(index);
			else if (adding == "MouseHold")
				ChangeTriggerToMouseHoldTrigger(index);
			else if (adding == "MouseHover")
				ChangeTriggerToMouseHoverTrigger(index);
			else if (adding == "MouseMovement")
				ChangeTriggerToMouseMovementTrigger(index);
			else if (adding == "Gamepad")
				ChangeTriggerToGamePadTrigger(index);
			else if (adding == "Touchpad")
				ChangeKeyTriggerToTouchPadTrigger(index);
		}

		private void ChanceTriggerToKeyTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (Key key in Enum.GetValues(typeof(Key)))
				foundFreeKey = CheckWichKeyForKeyTriggerToUse(foundFreeKey, key, index);
		}

		private bool CheckWichKeyForKeyTriggerToUse(bool foundFreeKey, Key key, int index)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfKeyIsAlreadyUsed(newTrigger, key, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			var keyTrigger = new KeyTrigger(key, State.Pressed);
			GetTriggersOfCommand()[index] = keyTrigger;
			return true;
		}

		private static bool CheckIfKeyIsAlreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(KeyTrigger))
				return keyAlreadyUsed;
			var keyTrigger = (KeyTrigger)newTrigger;
			if (keyTrigger.Key.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		private void ChangeTriggerToMouseTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
				foundFreeKey = CheckWhichButtonForMouseButtonTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWhichButtonForMouseButtonTriggerToUse(bool foundFreeKey, MouseButton button,
			int index)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfMouseButtonIsAlreadyUsed(newTrigger, button, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			var mouseButtonTrigger = new MouseButtonTrigger(button, State.Pressed);
			GetTriggersOfCommand()[index] = mouseButtonTrigger;
			return true;
		}

		private static bool CheckIfMouseButtonIsAlreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(MouseButtonTrigger))
				return keyAlreadyUsed;
			var mouseButtonTrigger = (MouseButtonTrigger)newTrigger;
			if (mouseButtonTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		private void ChangeTriggerToMouseDragDropTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
				foundFreeKey = CheckWhichButtonForMouseDragDropTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWhichButtonForMouseDragDropTriggerToUse(bool foundFreeKey, MouseButton button,
			int index)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfMouseDragDropButtonIsAlreadyUsed(newTrigger, button, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			var mouseDragDropTrigger = new MouseDragDropTrigger(new Rectangle(), button);
			GetTriggersOfCommand()[index] = mouseDragDropTrigger;
			return true;
		}

		private static bool CheckIfMouseDragDropButtonIsAlreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(MouseDragDropTrigger))
				return keyAlreadyUsed;
			var mouseDragDropTrigger = (MouseDragDropTrigger)newTrigger;
			if (mouseDragDropTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		private void ChangeTriggerToMouseHoldTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (MouseButton button in Enum.GetValues(typeof(MouseButton)))
				foundFreeKey = CheckWhichButtonForMouseHoldTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWhichButtonForMouseHoldTriggerToUse(bool foundFreeKey, MouseButton button,
			int index)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfMouseHoldButtonIsAlreadyUsed(newTrigger, button, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			var mouseHoldTrigger = new MouseHoldTrigger(new Rectangle(), 0.5f, button);
			GetTriggersOfCommand()[index] = mouseHoldTrigger;
			return true;
		}

		private static bool CheckIfMouseHoldButtonIsAlreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(MouseHoldTrigger))
				return keyAlreadyUsed;
			var mouseHoldTrigger = (MouseHoldTrigger)newTrigger;
			if (mouseHoldTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		private void ChangeTriggerToMouseHoverTrigger(int index)
		{
			var mouseHoldTrigger = new MouseHoverTrigger();
			inputEditorViewModel.availableCommands.GetAllTriggers(inputEditorViewModel.SelectedCommand)
				[index] = mouseHoldTrigger;
		}

		private void ChangeTriggerToMouseMovementTrigger(int index)
		{
			var mouseMovementTrigger = new MouseMovementTrigger();
			inputEditorViewModel.availableCommands.GetAllTriggers(inputEditorViewModel.SelectedCommand)
				[index] = mouseMovementTrigger;
		}

		private void ChangeTriggerToGamePadTrigger(int index)
		{
			bool foundFreeKey = false;
			foreach (GamePadButton button in Enum.GetValues(typeof(GamePadButton)))
				foundFreeKey = CheckWhichButtonForGamePadTriggerToUse(foundFreeKey, button, index);
		}

		private bool CheckWhichButtonForGamePadTriggerToUse(bool foundFreeKey, GamePadButton button,
			int index)
		{
			if (foundFreeKey)
				return true;
			bool keyAlreadyUsed = false;
			foreach (Trigger newTrigger in GetTriggersOfCommand())
				keyAlreadyUsed = CheckIfGamePadButtonIsALreadyUsed(newTrigger, button, keyAlreadyUsed);
			if (keyAlreadyUsed)
				return false;
			var mouseButtonTrigger = new GamePadButtonTrigger(button, State.Pressed);
			GetTriggersOfCommand()[index] = mouseButtonTrigger;
			return true;
		}

		private static bool CheckIfGamePadButtonIsALreadyUsed(Trigger newTrigger, object key,
			bool keyAlreadyUsed)
		{
			if (newTrigger.GetType() != typeof(GamePadButtonTrigger))
				return keyAlreadyUsed;
			var mouseButtonTrigger = (GamePadButtonTrigger)newTrigger;
			if (mouseButtonTrigger.Button.ToString() == key.ToString())
				keyAlreadyUsed = true;
			return keyAlreadyUsed;
		}

		private void ChangeKeyTriggerToTouchPadTrigger(int index)
		{
			var touchpadTrigger = new TouchPressTrigger(State.Pressed);
			inputEditorViewModel.availableCommands.GetAllTriggers(inputEditorViewModel.SelectedCommand)
				[index] = touchpadTrigger;
		}

		private void ChangeMouseTriggerTypeInCommandList(string adding, string key, Trigger trigger)
		{
			var newMouseTrigger = (MouseButtonTrigger)trigger;
			if (newMouseTrigger.Button.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeMouseDragDropTriggerTypeInCommandList(string adding, Trigger trigger)
		{
			CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeMouseHoldTriggerTypeInCommandList(string adding, string key, Trigger trigger)
		{
			var newMouseTrigger = (MouseHoldTrigger)trigger;
			if (newMouseTrigger.Button.ToString() == key)
				CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeMouseHoverTriggerTypeInCommandList(string adding, Trigger trigger)
		{
			CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeMouseMovementTriggerTypeInCommandList(string adding, Trigger trigger)
		{
			CheckWhatTriggerType(adding, trigger);
		}

		private void ChangeGamePadTriggerTypeInCommandList(string adding, string key,
			Trigger trigger)
		{
			var newMouseTrigger = (GamePadButtonTrigger)trigger;
			if (newMouseTrigger.Button.ToString() == key)
				CheckWhatTriggerType(adding, trigger); //ncrunch: no coverage
		}

		public void ChangeExistingTypeInList(string adding)
		{
			for (int i = 0; i < GetTriggersOfCommand().Count; i++)
				CheckWhatTriggerType(adding, GetTriggersOfCommand()[i]);
			inputEditorViewModel.UpdateTriggerList(inputEditorViewModel.SelectedCommand);
		}
	}
}