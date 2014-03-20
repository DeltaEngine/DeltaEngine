using System;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	public class InputKeyAndButtonEditor
	{
		//ncrunch: no coverage start
		public InputKeyAndButtonEditor(InputEditorViewModel inputEditorViewModel)
		{
			this.inputEditorViewModel = inputEditorViewModel;
		}

		private readonly InputEditorViewModel inputEditorViewModel;

		public void ChangeExistingKeyInList(string adding, string removing)
		{
			if (ListAlreadyHasTrigger(adding))
				return;

			foreach (var trigger in AvailableCommands.GetAllTriggers(SelectedCommand))
				CheckWichTriggerToChange(adding, removing, trigger);
		}

		private static void CheckWichTriggerToChange(string adding, string removing, Trigger trigger)
		{
			if (trigger.GetType() == typeof(KeyTrigger))
				ChangeKeyButtonInCommandList(adding, removing, trigger);
			if (trigger.GetType() == typeof(MouseButtonTrigger))
				ChangeMouseButtonInCommandList(adding, removing, trigger); //ncrunch: no coverage
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
				ChangeGamepadButtonInCommandList(adding, removing, trigger);
		}

		private String SelectedCommand
		{
			get { return inputEditorViewModel.SelectedCommand; }
		}
		private CommandList AvailableCommands
		{
			get { return inputEditorViewModel.availableCommands; }
		}

		private bool ListAlreadyHasTrigger(string adding)
		{
			isAlreadyInList = false;
			foreach (var trigger in AvailableCommands.GetAllTriggers(SelectedCommand))
				isAlreadyInList = CheckTriggerTypeOfAlreadyHas(adding, trigger);

			if (isAlreadyInList)
				inputEditorViewModel.UpdateTriggerList(SelectedCommand);
			return isAlreadyInList;
		}

		private bool isAlreadyInList;

		private bool CheckTriggerTypeOfAlreadyHas(string adding, Trigger trigger)
		{
			if (trigger.GetType() == typeof(KeyTrigger))
				isAlreadyInList = CheckIfKeyTriggerIsInList(adding, trigger);
			if (trigger.GetType() == typeof(MouseButtonTrigger))
				isAlreadyInList = CheckIfMouseTriggerIsInList(adding, trigger); //ncrunch: no coverage
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
				isAlreadyInList = CheckIfGamepadTriggerIsInList(adding, trigger);
			return isAlreadyInList;
		}

		private bool CheckIfKeyTriggerIsInList(string adding, Trigger trigger)
		{
			var keyTrigger = (KeyTrigger)trigger;
			if (keyTrigger.Key.ToString() == adding)
				isAlreadyInList = true;
			return isAlreadyInList;
		}

		private bool CheckIfMouseTriggerIsInList(string adding, Trigger trigger)
		{
			var mouseButtonTrigger = (MouseButtonTrigger)trigger;
			if (mouseButtonTrigger.Button.ToString() == adding)
				isAlreadyInList = true;
			return isAlreadyInList;
		}

		private bool CheckIfGamepadTriggerIsInList(string adding, Trigger trigger)
		{
			var gamePadButtonTrigger = (GamePadButtonTrigger)trigger;
			if (gamePadButtonTrigger.Button.ToString() == adding)
				isAlreadyInList = true;
			return isAlreadyInList;
		}

		private static void ChangeKeyButtonInCommandList(string adding, string removing,
			Trigger trigger)
		{
			var newKeyTrigger = (KeyTrigger)trigger;
			if (newKeyTrigger.Key.ToString() == removing)
				newKeyTrigger.Key = (Key)Enum.Parse(typeof(Key), adding);
		}

		private static void ChangeMouseButtonInCommandList(string adding, string removing,
			Trigger trigger)
		{
			var mouseButtonTrigger = (MouseButtonTrigger)trigger;
			if (mouseButtonTrigger.Button.ToString() == removing)
			{
				mouseButtonTrigger.Dispose();
				mouseButtonTrigger = new MouseButtonTrigger((MouseButton)Enum.Parse(typeof(MouseButton),
					adding));
			}
		}

		private static void ChangeGamepadButtonInCommandList(string adding, string removing,
			Trigger trigger)
		{
			var gamepadButtonTrigger = (GamePadButtonTrigger)trigger;
			if (gamepadButtonTrigger.Button.ToString() == removing)
				gamepadButtonTrigger.Button = (GamePadButton)Enum.Parse(typeof(GamePadButton), adding);
		}
	}
}