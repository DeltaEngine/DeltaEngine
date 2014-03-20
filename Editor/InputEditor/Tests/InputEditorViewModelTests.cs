using System;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.InputEditor.Tests
{
	public class InputEditorViewModelTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void Setup()
		{
			inputModel = new InputEditorViewModel(new MockService("Joey", "LogoApp"));
		}

		private InputEditorViewModel inputModel;

		[Test, STAThread]
		public void ClickAddCommandWithoutInput()
		{
			inputModel.AddCommand.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.NumberOfCommands);
		}

		public void AddCommandToList()
		{
			inputModel.NewCommand = "fire";
			inputModel.AddCommand.Execute(null);
			Assert.AreEqual(1, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread]
		public void AddTriggerToListWithoutCommand()
		{
			AddCommandToList();
			inputModel.AddTrigger.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.GetAllTriggers("fire").Count);
		}

		public void AddTriggerToList()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.AddTrigger.Execute(null);
			Assert.AreEqual(1, inputModel.availableCommands.GetAllTriggers("fire").Count);
		}

		[Test, STAThread, Category("Slow")]
		public void Add2TriggersToList()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.AddTrigger.Execute(null);
			inputModel.AddTrigger.Execute(null);
			Assert.AreEqual(2, inputModel.availableCommands.GetAllTriggers("fire").Count);
		}

		[Test, STAThread, Category("Slow")]
		public void Add2CommandToListAndSwichBetweenThem()
		{
			inputModel.NewCommand = "fire";
			inputModel.AddCommand.Execute(null);
			inputModel.NewCommand = "jump";
			inputModel.AddCommand.Execute(null);
			inputModel.SelectedCommand = "fire";
			inputModel.AddTrigger.Execute(null);
			inputModel.SelectedCommand = "jump";
			inputModel.SelectedCommand = "fire";
			Assert.AreEqual(2, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread]
		public void RemoveACommand()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.RemoveCommand.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread]
		public void EditACommand()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.NewCommand = "jump";
			inputModel.EditCommand.Execute(null);
			Assert.AreEqual("jump", inputModel.CommandList[0]);
		}

		[Test, STAThread, Category("Slow")]
		public void RemoveATrigger()
		{
			AddTriggerToList();
			inputModel.SelectedTrigger = inputModel.TriggerList[0];
			inputModel.RemoveTrigger.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.GetAllTriggers("fire").Count);
		}

		[Test, STAThread]
		public void RemoveANonExistingCommand()
		{
			inputModel.RemoveCommand.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread]
		public void EditANonExistingCommand()
		{
			inputModel.EditCommand.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread]
		public void RemoveANonExistingTrigger()
		{
			inputModel.RemoveTrigger.Execute(null);
			Assert.AreEqual(0, inputModel.availableCommands.NumberOfCommands);
		}

		[Test, STAThread, Category("Slow")]
		public void SaveInputAsXml()
		{
			AddCommandToList();
			inputModel.SelectedCommand = "fire";
			inputModel.AddTrigger.Execute(null);
			inputModel.SaveAsXml("");
			inputModel.TriggerList[0].TriggerType.SelectedItem = "Gamepad";
			inputModel.TriggerList[0].TriggerKey.SelectedItem = "B";
			inputModel.SaveAsXml("");
		}
	}
}