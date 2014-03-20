using System.Linq;
using DeltaEngine.Input;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.InputEditor.Tests
{
	public class CommandListTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Initialize()
		{
			commandList = new CommandList();
		}

		private CommandList commandList;

		[Test]
		public void CheckStatusOfEmptyList()
		{
			commandList.GetCommands();
			Assert.AreEqual(0, commandList.NumberOfCommands);
			Assert.AreEqual(0, commandList.GetAllTriggers("Jump").Count);
		}

		[Test]
		public void AddTriggerOfNonExistingCommand()
		{
			Assert.Throws<CommandList.CommandDoesNotExist>(
				() => commandList.AddTrigger("Jump", MouseButton.Left, State.Pressed));
			CheckStatusOfEmptyList();
		}

		[Test]
		public void AddFireCommandWithAllPossibleTrigger()
		{
			commandList.AddCommand("Fire");
			Assert.AreEqual(1, commandList.NumberOfCommands);
			commandList.AddTrigger("Fire", MouseButton.Left, State.Pressed);
			commandList.AddTrigger("Fire", Key.B, State.Pressed);
			commandList.AddTrigger("Fire", GamePadButton.B, State.Pressed);
			commandList.AddTrigger("Fire", State.Pressed);
			Assert.AreEqual(4, commandList.GetNumberOfTriggers("Fire"));
		}

		[Test]
		public void AddAndRemoveCommand()
		{
			commandList.AddCommand("Click");
			Assert.AreEqual(1, commandList.NumberOfCommands);
			commandList.RemoveCommand("Click");
			Assert.AreEqual(0, commandList.NumberOfCommands);
		}

		[Test]
		public void AddTwoCommandsWithTriggers()
		{
			commandList.AddCommand("Fire");
			commandList.AddCommand("Jump");
			Assert.AreEqual(2, commandList.NumberOfCommands);
			commandList.AddTrigger("Fire", MouseButton.Middle, State.Pressed);
			Assert.AreEqual(1, commandList.GetNumberOfTriggers("Fire"));
			commandList.AddTrigger("Jump", MouseButton.Right, State.Pressed);
			Assert.AreEqual(1, commandList.GetNumberOfTriggers("Jump"));
		}

		[Test]
		public void RemoveAllTriggersOfFireCommand()
		{
			AddFireCommandWithAllPossibleTrigger();
			commandList.RemoveTrigger("Fire", MouseButton.Left, State.Pressed);
			commandList.RemoveTrigger("Fire", Key.B, State.Pressed);
			commandList.RemoveTrigger("Fire", GamePadButton.B, State.Pressed);
			commandList.RemoveTrigger("Fire", State.Pressed);
			Assert.AreEqual(4, commandList.GetNumberOfTriggers("Fire"));
		}

		[Test]
		public void GetTriggerListWithoutCommand()
		{
			commandList.GetAllTriggers(null);
			Assert.AreEqual(0, commandList.GetNumberOfTriggers("Fire"));
		}

		[Test]
		public void EditNonExistingCommand()
		{
			commandList.EditCommand("Test", "jump");
			Assert.AreEqual(0, commandList.NumberOfCommands);
		}

		[Test]
		public void AddAnExistingCommandShouldReturn()
		{
			commandList.AddCommand("MyCommand");
			commandList.AddCommand("MyCommand");
			Assert.AreEqual(1, commandList.NumberOfCommands);
		}

		[Test]
		public void EditCommand()
		{
			commandList.AddCommand("MyCommand");
			commandList.EditCommand("MyCommand", "MyNewCommand");
			Assert.AreEqual(1, commandList.NumberOfCommands);
			Assert.AreEqual("MyNewCommand", commandList.GetCommands().ElementAt(0));
		}
	}
}