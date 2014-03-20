using System;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.InputEditor.Tests
{
	[Category("Slow")]
	internal class InputKeyAndButtonEditorTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			inputModel = new InputEditorViewModel(new MockService("Joey", "LogoApp"));
		}

		private InputEditorViewModel inputModel;

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeKeyValueOfTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerKey.SelectedItem = "Enter";
			Assert.AreEqual(1, inputModel.TriggerList.Count);
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

		//[Test, STAThread, CloseAfterFirstFrame]
		//public void ChangeMouseButtonValueOfTrigger()
		//{
		//	AddTriggerToList();
		//	inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
		//	inputModel.TriggerList[0].TriggerKey.SelectedItem = "Right";
		//	Assert.AreEqual("Right", inputModel.TriggerList[0].TriggerKey.SelectedItem);
		//}

		[Test, STAThread, CloseAfterFirstFrame]
		public void ChangeGamepadButtonValueOfTrigger()
		{
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[0].TriggerKey.SelectedItem = "B";
			Assert.AreEqual("B", inputModel.TriggerList[0].TriggerKey.SelectedItem);
		}

		[Test, STAThread, CloseAfterFirstFrame]
		public void Change2ValuesOfKeyTriggerToSameKey()
		{
			AddTriggerToList();
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerKey.SelectedItem = "Enter";
			inputModel.TriggerList[1].TriggerKey.SelectedItem = "Enter";
			Assert.AreEqual("Enter", inputModel.TriggerList[0].TriggerKey.SelectedItem);
			Assert.AreEqual("Backspace", inputModel.TriggerList[1].TriggerKey.SelectedItem);
		}

		//[Test, STAThread, CloseAfterFirstFrame]
		//public void Change2ValuesOfMouseButtonTriggerToSameKey()
		//{
		//	AddTriggerToList();
		//	AddTriggerToList();
		//	inputModel.TriggerList[0].TriggerType.SelectedItem = "Mouse";
		//	inputModel.TriggerList[1].TriggerType.SelectedItem = "Mouse";
		//	inputModel.TriggerList[0].TriggerKey.SelectedItem = "Right";
		//	inputModel.TriggerList[1].TriggerKey.SelectedItem = "Right";
		//	Assert.AreEqual("Right", inputModel.TriggerList[0].TriggerKey.SelectedItem);
		//	Assert.AreEqual("Middle", inputModel.TriggerList[1].TriggerKey.SelectedItem);
		//}

		[Test, STAThread, CloseAfterFirstFrame]
		public void Change2ValuesOfGamePadTriggerToSameKey()
		{
			AddTriggerToList();
			AddTriggerToList();
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[1].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[0].TriggerKey.SelectedItem = "B";
			inputModel.TriggerList[1].TriggerKey.SelectedItem = "B";
			Assert.AreEqual("A", inputModel.TriggerList[0].TriggerKey.SelectedItem);
			Assert.AreEqual("B", inputModel.TriggerList[1].TriggerKey.SelectedItem);
		}
	}
}