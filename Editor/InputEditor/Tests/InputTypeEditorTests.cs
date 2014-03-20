using System;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.InputEditor.Tests
{
	[Category("Slow")]
	internal class InputTypeEditorTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			inputModel = new InputEditorViewModel(new MockService("Joey", "LogoApp"));
		}

		private InputEditorViewModel inputModel;

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeTypeOfTriggerToMouse()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
			var trigger = (MouseButtonTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressed", trigger.State.ToString());
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
		public void ChangeTypeOfTriggerToGamePad()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			var trigger = (GamePadButtonTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressed", trigger.State.ToString());
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeTypeOfTriggerToTouch()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Touchpad";
			var trigger = (TouchPressTrigger)inputModel.availableCommands.GetAllTriggers("fire")[0];
			Assert.AreEqual("Pressed", trigger.State.ToString());
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangingTypeMultipleTimes()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Keyboard";
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Touchpad";
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Keyboard";
			Assert.AreEqual("Keyboard", inputModel.TriggerList[0].TriggerType.SelectedItem.ToString());
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangingTypeOf2TriggerToKey()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
			AddTriggerToList();
			inputModel.TriggerList[1].TriggerType.SelectedItem = "Mouse";
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Keyboard";
			inputModel.TriggerList[1].TriggerType.SelectedItem = "Keyboard";
			Assert.AreEqual("Keyboard", inputModel.TriggerList[0].TriggerType.SelectedItem.ToString());
		}
	}
}