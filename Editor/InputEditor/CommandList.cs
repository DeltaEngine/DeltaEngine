using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	/// <summary>
	/// Contains the List of Commands and it´s triggers, and can edit this list.
	/// </summary>
	public class CommandList
	{
		public CommandList()
		{
			commands = new Dictionary<string, Command>();
		}

		private readonly Dictionary<string, Command> commands;

		public int NumberOfCommands
		{
			get { return commands.Count; }
		}

		public IEnumerable<string> GetCommands()
		{
			return commands.Keys;
		}

		public int GetNumberOfTriggers(string commandName)
		{
			return GetAllTriggers(commandName).Count;
		}

		public List<Trigger> GetAllTriggers(string commandName)
		{
			if (commandName == null)
				return new List<Trigger>();
			if (commands.ContainsKey(commandName))
				return commands[commandName].GetTriggers();
			return new List<Trigger>();
		}

		public void AddCommand(string commandName)
		{
			if (commands.ContainsKey(commandName))
				return;
			commands.Add(commandName, new Command(() => {}));
		}

		public void AddTrigger(string commandName, MouseButton mousButton, State buttonState)
		{
			Command c = GetCommandByName(commandName);
			c.Add(new MouseButtonTrigger(mousButton, buttonState));
		}

		private Command GetCommandByName(string commandName)
		{
			if (!commands.ContainsKey(commandName))
				throw new CommandDoesNotExist(commandName);
			return commands[commandName];
		}

		public class CommandDoesNotExist : Exception
		{
			public CommandDoesNotExist(string commandName)
				: base(commandName) {}
		}

		public void AddTrigger(string commandName, Key key, State keyState)
		{
			Command c = GetCommandByName(commandName);
			c.Add(new KeyTrigger(key, keyState));
		}

		public void AddTrigger(string commandName, GamePadButton gamePadButton, State gamePadState)
		{
			Command c = GetCommandByName(commandName);
			c.Add(new GamePadButtonTrigger(gamePadButton, gamePadState));
		}

		public void AddTrigger(string commandName, State touchState)
		{
			Command c = GetCommandByName(commandName);
			c.Add(new TouchPressTrigger(touchState));
		}

		public void RemoveCommand(string commandName)
		{
			commands.Remove(commandName);
		}

		public void RemoveTrigger(string commandName, MouseButton mousButton, State buttonState)
		{
			List<Trigger> commandTriggers = GetAllTriggers(commandName);
			commandTriggers.Remove(new MouseButtonTrigger(mousButton, buttonState));
		}

		public void RemoveTrigger(string commandName, Key key, State buttonState)
		{
			List<Trigger> commandTriggers = GetAllTriggers(commandName);
			commandTriggers.Remove(new KeyTrigger(key, buttonState));
		}

		public void RemoveTrigger(string commandName, GamePadButton gamePadButton, State buttonState)
		{
			List<Trigger> commandTriggers = GetAllTriggers(commandName);
			commandTriggers.Remove(new GamePadButtonTrigger(gamePadButton, buttonState));
		}

		public void RemoveTrigger(string commandName, State buttonState)
		{
			List<Trigger> commandTriggers = GetAllTriggers(commandName);
			commandTriggers.Remove(new TouchPressTrigger(buttonState));
		}

		public void EditCommand(string currentCommandName, string newCommandName)
		{
			if (!commands.ContainsKey(currentCommandName) || commands.ContainsKey(newCommandName))
				return;
			Command triggersOfCommand = commands[currentCommandName];
			RemoveCommand(currentCommandName);
			commands.Add(newCommandName, triggersOfCommand);
		}
	}
}