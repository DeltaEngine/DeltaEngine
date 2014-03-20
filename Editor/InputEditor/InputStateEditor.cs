using System;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputStateEditor
	{
		public InputStateEditor(InputEditorViewModel inputEditorViewModel)
		{
			this.inputEditorViewModel = inputEditorViewModel;
		}

		private readonly InputEditorViewModel inputEditorViewModel;

		//ncrunch: no coverage start
		public void ChangeExistingStateInList(string adding, string key)
		{
			foreach (var trigger in AvailableCommands.GetAllTriggers(SelectedCommand))
				CheckWichTriggerTypeToChange(adding, key, trigger);
		}

		private String SelectedCommand
		{
			get { return inputEditorViewModel.SelectedCommand; }
		}
		private CommandList AvailableCommands
		{
			get { return inputEditorViewModel.availableCommands; }
		}

		private static void CheckWichTriggerTypeToChange(string adding, string key, Trigger trigger)
		{
			if (trigger.GetType() == typeof(KeyTrigger))
				ChangeKeyTriggerStateInCommandList(adding, key, trigger);
			//if (trigger.GetType() == typeof(MouseButtonTrigger))
			//	ChangeMouseButtonTriggerStateInCommandList(adding, key, trigger);
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
				ChangeGamepadButtonTriggerStateInCommandList(adding, key, trigger);
		}

		private static void ChangeKeyTriggerStateInCommandList(string adding, string key,
			Trigger trigger)
		{
			var newKeyTrigger = (KeyTrigger)trigger;
			if (newKeyTrigger.Key.ToString() == key)
				newKeyTrigger.State = (State)Enum.Parse(typeof(State), adding);
		}

		//private static void ChangeMouseButtonTriggerStateInCommandList(string adding, string key,
		//	Trigger trigger)
		//{
		//	var newMouseTrigger = (MouseButtonTrigger)trigger;
		//	if (newMouseTrigger.Button.ToString() == key)
		//		newMouseTrigger.State = (State)Enum.Parse(typeof(State), adding);
		//}

		private static void ChangeGamepadButtonTriggerStateInCommandList(string adding, string key,
			Trigger trigger)
		{
			var newGamepadTrigger = (GamePadButtonTrigger)trigger;
			if (newGamepadTrigger.Button.ToString() == key)
				newGamepadTrigger.State = (State)Enum.Parse(typeof(State), adding);
		}

		public void ChangeExistingStateInList(string adding)
		{
			foreach (var trigger in AvailableCommands.GetAllTriggers(SelectedCommand))
				if (trigger.GetType() == typeof(TouchPressTrigger))
					ChangeTouchpadButtonTriggerStateInCommandList(adding, trigger);
		}

		private static void ChangeTouchpadButtonTriggerStateInCommandList(string adding,
			Trigger trigger)
		{
			var newTouchpadTrigger = (TouchPressTrigger)trigger;
			newTouchpadTrigger.State = (State)Enum.Parse(typeof(State), adding);
		}
	}
}