using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content.Xml;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;

namespace DeltaEngine.Editor.InputEditor
{
	internal class InputSaverAndLoader
	{
		//ncrunch: no coverage start
		public void SaveInput(CommandList commandList, Service service)
		{
			var root = CreateMainRoot("InputCommands");
			foreach (var command in commandList.GetCommands())
				SetCommand(root, command, commandList.GetAllTriggers(command));
			var bytes = new XmlFile(root).ToMemoryStream().ToArray();
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add("InputCommands.xml", bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			var contentMetaData = metaDataCreator.CreateMetaDataFromInputData(bytes);
			service.UploadContent(contentMetaData, fileNameAndBytes);
		}

		private static XmlData CreateMainRoot(string filename)
		{
			var root = new XmlData(filename) { Name = "InputCommands" };
			return root;
		}

		private static void SetCommand(XmlData root, string command, IEnumerable<Trigger> triggers)
		{
			var child = new XmlData("Command");
			child.AddAttribute("Name", command);
			foreach (Trigger trigger in triggers)
				SetTrigger(trigger, child);
			root.AddChild(child);
		}

		private static void SetTrigger(Trigger trigger, XmlData xmlData)
		{
			XmlData child = null;
			if (trigger.GetType() == typeof(KeyTrigger))
			{
				var keyTrigger = trigger as KeyTrigger;
				child = new XmlData("KeyTrigger") { Value = keyTrigger.Key + " " + keyTrigger.State };
			}
			if (trigger.GetType() == typeof(MouseButtonTrigger))
			{
				var mouseButtonTrigger = trigger as MouseButtonTrigger;
				child = new XmlData("MouseButtonTrigger");
				child.Value = mouseButtonTrigger.Button + " " + mouseButtonTrigger.State;
			}
			if (trigger.GetType() == typeof(MouseDragDropTrigger))
			{
				var mouseDragDropTrigger = trigger as MouseDragDropTrigger;
				child = new XmlData("MouseDragAndDropTrigger");
				child.Value = mouseDragDropTrigger.Button.ToString();
			}
			if (trigger.GetType() == typeof(MouseHoldTrigger))
			{
				var mouseHoldTrigger = trigger as MouseHoldTrigger;
				child = new XmlData("MouseHoldTrigger");
				child.Value = mouseHoldTrigger.Button.ToString();
			}
			if (trigger.GetType() == typeof(MouseHoverTrigger))
				child = new XmlData("MouseHoverTrigger");
			if (trigger.GetType() == typeof(MouseMovementTrigger))
				child = new XmlData("MouseMovementTrigger");
			if (trigger.GetType() == typeof(GamePadButtonTrigger))
			{
				var gamePadButtonTrigger = trigger as GamePadButtonTrigger;
				child = new XmlData("GamePadButtonTrigger");
				child.Value = gamePadButtonTrigger.Button + " " + gamePadButtonTrigger.State;
			}
			if (trigger.GetType() == typeof(TouchPressTrigger))
			{
				var touchPressTrigger = trigger as TouchPressTrigger;
				child = new XmlData("TouchPressTrigger") { Value = touchPressTrigger.State.ToString() };
			}
			xmlData.AddChild(child);
		}

		public static void LoadInput(InputEditorViewModel editor)
		{
			var metadataXml =
				XDocument.Load(Path.Combine("Content/" + editor.service.ProjectName, "InputCommands.xml"));
			foreach (XElement contentElement in metadataXml.Root.Elements())
			{
				var commandName = contentElement.FirstAttribute;
				editor.NewCommand = commandName.Value;
				editor.AddNewCommand();
				foreach (var element in contentElement.Elements())
				{
					if (element.Name == "KeyTrigger")
						LoadKeyCommand(element, editor, commandName.Value);
					if (element.Name == "MouseButtonTrigger")
						LoadMouseCommand(element, editor, commandName.Value);
					if (element.Name == "GamePadButtonTrigger")
						LoadGamepadCommand(element, editor, commandName.Value);
				}
			}
		}

		private static void LoadKeyCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var valueSplit = element.Value.Split(' ');
			editor.availableCommands.AddTrigger(commandName,
				((Key)Enum.Parse(typeof(Key), valueSplit[0])),
				((State)Enum.Parse(typeof(State), valueSplit[1])));
		}

		private static void LoadMouseCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var valueSplit = element.Value.Split(' ');
			editor.availableCommands.AddTrigger(commandName,
				((MouseButton)Enum.Parse(typeof(MouseButton), valueSplit[0])),
				((State)Enum.Parse(typeof(State), valueSplit[1])));
		}

		private static void LoadGamepadCommand(XElement element, InputEditorViewModel editor,
			string commandName)
		{
			var valueSplit = element.Value.Split(' ');
			editor.availableCommands.AddTrigger(commandName,
				((GamePadButton)Enum.Parse(typeof(GamePadButton), valueSplit[0])),
				((State)Enum.Parse(typeof(State), valueSplit[0])));
		}
	}
}