using System;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.InputEditor.Tests
{
	[Category("Slow")]
	internal class InputStateEditorTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			inputModel = new InputEditorViewModel(new MockService("Joey", "LogoApp"));
		}

		private InputEditorViewModel inputModel;

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeStateValueOfTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerState.SelectedItem = "Pressing";
			var trigger = (KeyTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressing", trigger.State.ToString());
		}

		private void AddTriggerToList()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.AddTrigger.Execute(null);
		}

		private void AddCommandToList()
		{
			inputModel.NewCommand = "fire";
			inputModel.AddCommand.Execute(null);
			Assert.AreEqual(1, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeStateForMouseTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
			inputModel.TriggerList[0].TriggerState.SelectedItem = "Pressing";
			var trigger = (MouseButtonTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressing", trigger.State.ToString());
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeStateForGamepadTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[0].TriggerState.SelectedItem = "Pressing";
			var trigger = (GamePadButtonTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressing", trigger.State.ToString());
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeStateForTouchpadTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Touchpad";
			inputModel.TriggerList[0].TriggerState.SelectedItem = "Pressing";
			var trigger = (TouchPressTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressing", trigger.State.ToString());
		}
	}
}